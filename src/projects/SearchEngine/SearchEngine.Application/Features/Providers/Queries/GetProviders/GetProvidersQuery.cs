using AutoMapper;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
using MediatR;
using SearchEngine.Application.Services.ContentProviders;
using System.Net;

namespace SearchEngine.Application.Features.Providers.Queries.GetProviders;

public class GetProvidersQuery : IRequest<CustomResponseDto<List<GetProvidersResponse>>>
{
    public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, CustomResponseDto<List<GetProvidersResponse>>>
    {
        private readonly IContentProviderFactory _providerFactory;
        private readonly IMapper _mapper;

        public GetProvidersQueryHandler(IContentProviderFactory providerFactory, IMapper mapper)
        {
            _providerFactory = providerFactory;
            _mapper = mapper;
        }

        public async Task<CustomResponseDto<List<GetProvidersResponse>>> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
        {
            var providers = _providerFactory.GetProviders();
            var allContents = new List<Content>();

            var tasks = providers.Select(async provider =>
            {
                try
                {
                    return await provider.GetContentsAsync(cancellationToken);
                }
                catch (Exception)
                {
                    return new List<Content>();
                }
            }).ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var contents in results)
            {
                allContents.AddRange(contents);
            }

            var response = _mapper.Map<List<GetProvidersResponse>>(allContents);

            return CustomResponseDto<List<GetProvidersResponse>>.Success((int)HttpStatusCode.OK, response, true);
        }
    }
}

