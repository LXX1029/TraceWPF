namespace TraceWPF.Domain.Interfaces
{
    using TraceWPF.Domain.Models;

    /// <summary>
    /// 数据服务接口，定义对持久化层的基本数据访问操作。
    /// Data service interface that defines basic data access operations for the persistence layer.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// 获取最新的欢迎消息。
        /// Retrieves the latest welcome message.
        /// </summary>
        /// <returns>欢迎消息实体 / The welcome message entity.</returns>
        Message GetWelcomeMessage();
    }
}

