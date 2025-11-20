# Arama Motoru Servisi – README

## Hızlı Başlangıç

```bash
git clone <https://github.com/demirkankcr/SearchEngine.git>
cd SearchEngine
docker-compose up -d 
```

**Servisler hazır olunca (30-60 saniye):**
- ✅ **API & Swagger:** http://localhost:5000/swagger
- ✅ **Dashboard UI:** http://localhost:5001
- ✅ **Hangfire:** http://localhost:5000/hangfire
- ✅ **Redis Commander:** http://localhost:8081
- 🟡 **ElasticSearch:** http://localhost:9200 (Konteyner hazır, proje büyüdüğünde aktif edilebilir)

**Not:** 
- Docker üzerinde api projesi ilk ayağa kalktığında veya uygulanmamış migration olduğunda migration'lar otomatik uygulanır. Hem Hangfire Hem Content için (manuel dotnet ef database update gerekmez)
- Tüm servisler (PostgreSQL, Redis, ElasticSearch, API, UI) otomatik başlar

## API Dokümantasyonu
İki endpoint için hem swagger hem dashboard üzerinden kontrolü sağlabilir. Alternatif olarak Postman, cURL gibi araçlarla da test edebilirsiniz.

### 🔍 `GET /api/contents/search`
İçerikleri aramak, filtrelemek ve sıralamak için kullanılır.

**Örnek İstek (cURL):**
```bash
curl -X GET "http://localhost:5000/api/contents/search?Keyword=AI&SortBy=relevancedesc&Page=0&PageSize=10" -H "accept: application/json"
```

**Parametreler:**
| Parametre | Tip | Açıklama | Örnek |
|-----------|-----|----------|-------|
| `Keyword` | string | Aranacak kelime | `Go`, `Eğitim` |
| `ContentType` | enum | `Video` veya `Text` | `1` (Video), `2` (Text) |
| `SortBy` | string | Sıralama kriteri | `relevancedesc`, `datedesc`, `popularitydesc` |
| `Page` | int | Sayfa numarası (0'dan başlar) | `0` |
| `PageSize` | int | Sayfadaki kayıt sayısı | `10` |

**Önemli Bilgiler:**
- **Rate Limit:** 10 saniyede en fazla 25 istek atabilirsiniz (`searchLimiter`).
- **Cache:** Sonuçlar Redis'te saklanır. Aynı sorguyu tekrar atarsanız veritabanına gitmeden cache'den döner.

---

### 🔄 `POST /api/contents/sync`
Dış provider'lardan (JSON/XML) verileri çeker ve veritabanını günceller.

**Örnek İstek (cURL):**
```bash
curl -X POST "http://localhost:5000/api/contents/sync" -H "accept: */*" -d ""
```

**Önemli Bilgiler:**
- **Rate Limit:** 1 dakikada en fazla 5 istek atabilirsiniz (`syncLimiter`).
- **Otomasyon:** Bu komutu elle çalıştırmanıza gerek yoktur; Hangfire her dakika otomatik çalıştırır.
- **Cache Temizliği:** Bu komut başarılı olursa, Redis'teki tüm arama cache'leri silinir (Invalidation).

---

## 📐 Proje Mimarisi ve İşleyiş (End-to-End Flow)

Proje, veriyi dış dünyadan alıp son kullanıcıya sunana kadar 4 ana aşamadan geçer.

### 1. Veri Toplama (Ingestion) & Resilience
Sistem, farklı formatlardaki (JSON, XML) dış kaynaklardan (Provider) veri çeker.
- **`ContentProviderFactory` (Factory Pattern):** Hangi provider'dan (JSON/XML) istek yapılacağını dinamik olarak belirler. Kod içinde `if-else` karmaşası yerine temiz bir nesne üretimi sağlar.
- **`Polly` (Resilience):** Dış servisler cevap vermezse veya hata fırlatırsa, sistem çökmez; **Retry Policy** ile belirli aralıklarla tekrar dener. Eğer servis tamamen çökmüşse **Circuit Breaker** devreye girer ve sistemi korur.
- **Standardizasyon:** Gelen ham veri (raw data), ortak bir `Content` modeline (Video veya Text) dönüştürülür.

### 2. Puanlama (Scoring) & Strategy Pattern
Veriler veritabanına yazılmadan önce esnek bir puanlama motorundan geçer.
- **`Strategy Pattern`**: Puanlama mantığı `IScoringStrategy` interface'i üzerinden soyutlanmıştır. `VideoScoringStrategy` ve `TextScoringStrategy` sınıfları farklı algoritmalar çalıştırır.
- **Avantajı:** Yarın "Podcast" diye yeni bir içerik türü gelirse, sadece yeni bir strateji sınıfı yazmak yeterlidir; ana kodu değiştirmeye gerek kalmaz (**Open/Closed Principle**).
- **Formül:** `(Temel Puan * Katsayı) + Güncellik + Etkileşim` hesaplanır.

### 3. Veri Saklama & Otomasyon (Persistence & Background Jobs)
- **PostgreSQL & EF Core:** Puanlanmış ve standardize edilmiş veri veritabanına "Upsert" (varsa güncelle, yoksa ekle) mantığıyla kaydedilir. **TPH (Table Per Hierarchy)** deseni kullanılarak tüm içerik tipleri performanslı bir şekilde tek tabloda tutulur.
- **Hangfire (Zamanlanmış Görevler):** 
  - **Sıklık:** Her dakika (`Cron.Minutely`) çalışan bir job vardır.
  - **Görevi:** Otomatik olarak provider'ları tarar, yeni içerik varsa çeker ve veritabanını günceller.
  - **Cache Temizliği:** Job başarıyla biterse, Redis'teki eski arama sonuçlarını siler.

### 4. Arama ve Sunum (Serving) & Caching
Kullanıcı API veya Dashboard üzerinden arama yaptığında:
1. **Redis (Distributed Cache):** Önce Redis'e bakar.
   - **HIT:** Direkt cache'den döner (Milisaniyeler sürer).
   - **MISS:** Veritabanına gider, sonucu bulur ve Redis'e yazar.
   - **Verimlilik:** Eğer provider'lardan yeni veri gelmediyse, 60 dakika boyunca DB'ye hiç yük bindirmeden aynı veriyi Redis'ten döneriz.
2. **Dashboard (UI):** API'den gelen bu veriyi Bootstrap ile hazırlanmış kullanıcı dostu bir tabloda gösterir.

---

## Teknoloji Tercihleri (neden böyle?)

1. **.NET 8 & Clean Architecture** – Katmanlı yapı + MediatR ile büyürken kodu dağıtmadan ilerledim. Böyle bir proje için biraz fazla karmaşık yapı olabilir ama hem ölçeklenebilir bir yapı istenmesi hem de 
genel olarak kendi projelerimde kullanmak üzere oluşturduğum clean architecture templatesi olduğu için daha rahat bir biçimde geliştirme sağlamış oldum.

2. **EF Core 8 & PostgreSQL** – Table Per Hierarchy (TPH) inheritance pattern ile Content, TextContent ve VideoContent entity'leri tek tabloda tutuluyor. Bu yaklaşım, ortak alanların tekrarlanmasını önler ve polimorfik sorguları basitleştirir. Hangfire background job'ları da aynı veritabanını kullanarak ekstra altyapı gereksinimini ortadan kaldırıyor.

3. **MediatR Pipeline Behaviors** – Caching, validation, logging gibi işler handler içine gömülmedi; AOP mantığıyla ilerledim. Cross-cutting işlerin tek merkezden yönetilmesi kod tekrarını bitiriyor; decorator tabanlı alternatiflere göre daha okunabilir.

4. **Polly (Retry + Bulkhead)** – Provider API’leri 500/timeout verdiğinde otomatik retry ve eşzamanlı istek limiti var. Polly .NET ekosistemine en iyi entegre resiliency kütüphanesi; custom retry mekanizması yazmaya göre çok daha güvenilir ve test edilebilir.

5. **Hangfire** – Sync komutunu planlı çalıştırıp dashboard’dan takip edebilmek için.

6. **Redis + ICacheService (Fallback: MemoryCache)** – Redis cache entegrasyonu yapıldı; Redis kapalıysa veya bağlanamazsa otomatik olarak MemoryCache'e fallback yapıyor. ICacheService interface'i sayesinde cache implementasyonu değişikliği tek konfigürasyonla yönetiliyor. Search için ElasticSearch kullanmak bu proje kapsamında overengineering olurdu; mevcut veri hacmi ve arama gereksinimleri için PostgreSQL'in full-text search özellikleri yeterli. ElasticSearch eklemek hem docker configlerinin karmaşıklaşmasına hem de development/değerlendirme süreçlerinde gereksiz yavaşlığa sebep olacaktır.

7. **AutoMapper & FluentValidation** – DTO/Entity dönüşümleri ve request kontrolleri tekrar eden kod yazdırmıyor.

8. **Serilog (PostgreSQL sink)** – API ve job loglarını tek yerde topladım.

9. **ASP.NET Core MVC + Bootstrap 5** – Dashboard’u ince istemci yaptım; sadece API tüketip tablo render ediyor. Ayrı bir react projesi yapmak daha mantıklı olurda fakat yine proje çok dallanacağı ve node paketleri devreye gireceği için basit işlevsen bir mvc yaptım.

10. **xUnit & Moq** – Scoring ve provider senaryolarını güvence altına almak için standart test ekosistemi.

---

## Branch Günlüğü (özet)

- `feature/project-setup` – Clean Architecture iskeleti, core paketler, logging.
- `feature/content-entities` – Content/Text/Video entity’leri, enumlar, TPH konfigürasyonu.
- `feature/provider-integration` – JSON & XML provider’lar, factory, Polly policy’leri.
- `feature/data-persistence` – Scoring servisi, batch repository, Sync command, Hangfire job.
- `feature/search-api` – GetSearchContentsQuery, caching pipeline, rate limiting.
- `feature/dashboard` – MVC dashboard, filtre + pagination, Bootstrap arayüz.
- `feature/testing-docs` – Unit testler ve dokümantasyon düzeltmeleri.
- `feature/redis-integration` – Dockerize Redis, Cache Pipeline, Rate Limiting ve Redis Commander entegrasyonu.

---

## Dashboard & Testler

- `SearchEngine.UI`: HttpClient ile API’ye bağlanan, keyword/tür/sort filtreleri ve sayfalama sunan tek sayfa.
- Testler: Scoring stratejileri, ContentProviderFactory, GetSearchContents handler’ı için unit/handler senaryoları hazır.

---



## İsterler

| İster (Gereksinim) | Projede Karşılanan Çözüm |
|--------------------|---------------------------|
| **Farklı provider’lardan veri alınmalı (JSON + XML)** | JSON ve XML için ayrı provider servisleri, ortak interface ve provider factory ile tüm provider’lar standart forma dönüştürülüyor. |
| **Yeni provider kolay eklenebilir olmalı** | Clean Architecture + gevşek bağlı provider altyapısı; yeni provider eklemek için yalnızca ilgili provider service ve mapping eklemek yeterli. |
| **İstek limiti yönetimi (rate limit)** | ASP.NET rate limiting + Polly Bulkhead ile API ve provider çağrıları için eşzamanlı istek kontrolü. |
| **Veriler veritabanında saklanmalı** | PostgreSQL + EF Core (TPH) ile kalıcı veri saklama; batch upsert ile tutarlı veri akışı. |
| **Cache mekanizması önerilmeli** | Redis cache entegrasyonu; Redis kapalıysa MemoryCache'e otomatik fallback. |
| **Arama: keyword, tür filtresi, skor sıralama, pagination** | `GET /api/contents/search` endpoint’i tüm filtreler + sıralamalar + pagination destekliyor. |
| **Puanlama algoritması uygulanmalı** | Temel puan + içerik türü katsayısı + güncellik + etkileşim puanı tam formüle uygun hesaplanıyor. |
| **Planlı senkronizasyon job’ı** | Hangfire ile periyodik sync job; dashboard üzerinden izlenebilirlik. |
| **Dashboard gerekli** | MVC + Bootstrap 5 ile basit listeleme + filtreleme + sıralama sunan UI. |
| **Hata yönetimi** | Global exception middleware + Serilog logging + Polly retry ile tutarlı ve dayanıklı error handling. |
| **Test yazılmalı** | xUnit + Moq ile scoring, provider factory ve search handler için unit test senaryoları. |
| **API dokümantasyonu olmalı** | Swagger tamamen açık; endpoint parametreleri ve örnek cevapları içeriyor. |
