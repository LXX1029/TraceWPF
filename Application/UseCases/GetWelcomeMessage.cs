namespace TraceWPF.Application.UseCases
{
    using TraceWPF.Domain.Interfaces;
    using TraceWPF.Domain.Models;
    using TraceWPF.DI;

    /// <summary>
    /// 获取欢迎消息用例的实现类，通过 IDataService 从持久层获取最新的欢迎消息。
    /// Implementation of the "Get Welcome Message" use case; retrieves the latest welcome message from the persistence layer via IDataService.
    /// </summary>
    public class GetWelcomeMessage : IGetWelcomeMessage, ISingleton
    {
        /// <summary>
        /// 数据服务接口，用于访问持久化层。
        /// Data service interface for accessing the persistence layer.
        /// </summary>
        private readonly IDataService _dataService;

        /// <summary>
        /// 构造函数，通过 DI 注入数据服务。
        /// Constructor; injects the data service via DI.
        /// </summary>
        /// <param name="dataService">数据服务实例 / The data service instance.</param>
        public GetWelcomeMessage(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// 执行获取欢迎消息的逻辑，委托给 IDataService 实现。
        /// Executes the logic to retrieve the welcome message, delegating to the IDataService implementation.
        /// </summary>
        /// <returns>包含欢迎内容的消息实体 / A message entity containing the welcome content.</returns>
        public Message Execute()
        {
            return _dataService.GetWelcomeMessage();
        }
    }
}
