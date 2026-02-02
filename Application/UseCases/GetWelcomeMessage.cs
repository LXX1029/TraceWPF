namespace TraceWPF.Application.UseCases
{
    using TraceWPF.Domain.Interfaces;
    using TraceWPF.Domain.Models;
    using TraceWPF.DI;

    public class GetWelcomeMessage : IGetWelcomeMessage, ISingleton
    {
        private readonly IDataService _dataService;

        public GetWelcomeMessage(IDataService dataService)
        {
            _dataService = dataService;
        }

        public Message Execute()
        {
            return _dataService.GetWelcomeMessage();
        }
    }
}
