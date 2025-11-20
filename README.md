# Arama Motoru Servisi – README (Kısa Versiyon)

Bu repo, JSON ve XML provider’lardan gelen içerikleri toplayıp puanlayan, arama sonuçlarını sıralayan ve hem API hem dashboard üzerinden gösteren bir .NET 8 projesi. Aşağıdaki başlıklar en sık sorulan konuların kısa cevabı.

---

## Kurulum & Çalıştırma

1. master branchi üzerinden en güncel sürüme ulaşılabilir.
2. `dotnet restore`
3. `src/projects/SearchEngine/SearchEngine.API/appsettings.json` içinde `DefaultConnection` + Hangfire bağlantısını kişisel ortama uyarlanması
4. Migration:  
   `dotnet ef database update --project src/corepackages/Core.Persistence --startup-project src/projects/SearchEngine/SearchEngine.API`
5. API: `dotnet run --project src/projects/SearchEngine/SearchEngine.API` --5103 default port
6. Dashboard: `dotnet run --project src/projects/SearchEngine/SearchEngine.UI` --5213 default port
7. Testler: `dotnet test`
8. Hızlı erişim: Swagger `/swagger`, Hangfire `/hangfire`, Dashboard `/`

---

## API Dokümantasyonu
İki endpoint içinhem swagger hem dashboard üzerinden kontrolü sağlabilir.


### `GET /api/contents/search`
- Parametreler: `Keyword`, `ContentType`, `SortBy`, `Page`, `PageSize`, `DynamicQuery.Filter/Sort`
  - `SortBy`: `relevancedesc`, `relevanceasc`, `popularitydesc`, `popularityasc`, `datedesc`, `dateasc`
- Rate limit: 10 sn’de 25 istek (`searchLimiter`)
- Cache: Her kombinasyon `SearchContents(...)` key’iyle saklanır; sync sonrası `_cacheService.RemoveByPattern("^SearchContents\\(")` ile topluca silinir.


### `POST /api/contents/sync`
- Provider’lardan veri çeker, batch upsert yapar, skorları yeniden hesaplar.
- Rate limit: 1 dk’da 5 istek (`syncLimiter`)
- Çıktı: `{ success: true, message: "Sync completed." }`
- Hangfire job’ı aynı komutu periyodik koşturur, `[DisableConcurrentExecution]` sayesinde çakışma olmaz.

---

## Teknoloji Tercihleri (neden böyle?)

1. **.NET 8 & Clean Architecture** – Katmanlı yapı + MediatR ile büyürken kodu dağıtmadan ilerledim. Böyle bir proje için biraz fazla karmaşık yapı olabilir ama hem ölçeklenebilir bir yapı istenmesi hem de 
genel olarak kendi projelerimde kullanmak üzere oluşturduğum clean architecture templatesi olduğu için daha rahat bir biçimde geliştirme sağlamış oldum.
2. **EF Core 8 & PostgreSQL** – TPH ile Content/Text/Video tek tabloda, Hangfire da aynı DB’yi kullanıyor.
3. **MediatR Pipeline Behaviors** – Caching, validation, logging gibi işler handler içine gömülmedi; AOP mantığıyla ilerledim. Cross-cutting işlerin tek merkezden yönetilmesi kod tekrarını bitiriyor; decorator tabanlı alternatiflere göre daha okunabilir.
4. **Polly (Retry + Bulkhead)** – Provider API’leri 500/timeout verdiğinde otomatik retry ve eşzamanlı istek limiti var. Polly .NET ekosistemine en iyi entegre resiliency kütüphanesi; custom retry mekanizması yazmaya göre çok daha güvenilir ve test edilebilir.
5. **Hangfire** – Sync komutunu planlı çalıştırıp dashboard’dan takip edebilmek için.
6. **MemoryCache + ICacheService** – Okuma performansı için hızlı çözüm; interface sayesinde Redis’e geçiş tek konfigürasyon. burda caching için redis ve search için elk kullanmak daha mantıklı olurdu fakat bunlarla beraber docker configlerinin yapılmasıyla beraber hem development hem de değerlendirme kısmında yavaşlığa sebep olacaktır.
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

---



## Dashboard & Testler

- `SearchEngine.UI`: HttpClient ile API’ye bağlanan, keyword/tür/sort filtreleri ve sayfalama sunan tek sayfa.
- Testler: Scoring stratejileri, ContentProviderFactory, GetSearchContents handler’ı için unit/handler senaryoları hazır.

---
## Error Handling (Genel Yaklaşım)

Hata yönetimi için global exception middleware kullanıldı, böylece API tutarlı bir error response dönüyor. Provider çağrılarında Polly ile retry/bulkhead mekanizmaları devreye girerek transient hataların kullanıcıya yansımadan çözülmesi sağlandı. Ek olarak Serilog ile tüm hatalar merkezi olarak kaydedilerek izlenebilirlik artırıldı.

## İsterler

## İsterler

| İster (Gereksinim) | Projede Karşılanan Çözüm |
|--------------------|---------------------------|
| **Farklı provider’lardan veri alınmalı (JSON + XML)** | JSON ve XML için ayrı provider servisleri, ortak interface ve provider factory ile tüm provider’lar standart forma dönüştürülüyor. |
| **Yeni provider kolay eklenebilir olmalı** | Clean Architecture + gevşek bağlı provider altyapısı; yeni provider eklemek için yalnızca ilgili provider service ve mapping eklemek yeterli. |
| **İstek limiti yönetimi (rate limit)** | ASP.NET rate limiting + Polly Bulkhead ile API ve provider çağrıları için eşzamanlı istek kontrolü. |
| **Veriler veritabanında saklanmalı** | PostgreSQL + EF Core (TPH) ile kalıcı veri saklama; batch upsert ile tutarlı veri akışı. |
| **Cache mekanizması önerilmeli** | MemoryCache + ICacheService ile hızlı caching; Redis geçişine hazır yapı. |
| **Arama: keyword, tür filtresi, skor sıralama, pagination** | `GET /api/contents/search` endpoint’i tüm filtreler + sıralamalar + pagination destekliyor. |
| **Puanlama algoritması uygulanmalı** | Temel puan + içerik türü katsayısı + güncellik + etkileşim puanı tam formüle uygun hesaplanıyor. |
| **Planlı senkronizasyon job’ı** | Hangfire ile periyodik sync job; dashboard üzerinden izlenebilirlik. |
| **Dashboard gerekli** | MVC + Bootstrap 5 ile basit listeleme + filtreleme + sıralama sunan UI. |
| **Hata yönetimi** | Global exception middleware + Serilog logging + Polly retry ile tutarlı ve dayanıklı error handling. |
| **Test yazılmalı** | xUnit + Moq ile scoring, provider factory ve search handler için unit test senaryoları. |
| **API dokümantasyonu olmalı** | Swagger tamamen açık; endpoint parametreleri ve örnek cevapları içeriyor. |


## Yol Haritası

- Interface hazır olduğu için Redis/dağıtık cache’e geçmek kolay.
- ElasticSearch kullanılması search servisinde.
- Provider rate limit + auth ayarlarını merkezi yönetecek küçük bir config servisi.
- CrossCuttingConcers katmanında FluentValidation kolayca eklenebilir alt yapısı hazır
-.Dashboard’a sync durumunu gösteren ufak bildirimler.
- Log analizi için Grafana/Kibana entegrasyonu.


