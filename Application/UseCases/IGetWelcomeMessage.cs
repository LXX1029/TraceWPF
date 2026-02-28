namespace TraceWPF.Application.UseCases
{
    using TraceWPF.Domain.Models;

    /// <summary>
    /// 获取欢迎消息用例的接口定义。
    /// Interface definition for the "Get Welcome Message" use case.
    /// </summary>
    public interface IGetWelcomeMessage
    {
        /// <summary>
        /// 执行用例，返回欢迎消息。
        /// Executes the use case and returns a welcome message.
        /// </summary>
        /// <returns>欢迎消息实体 / The welcome message entity.</returns>
        Message Execute();
    }
}

