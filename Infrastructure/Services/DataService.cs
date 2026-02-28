namespace TraceWPF.Infrastructure.Services
{
    using System;
    using System.Linq;
    using SqlSugar;
    using TraceWPF.Domain.Interfaces;
    using TraceWPF.Domain.Models;
    using TraceWPF.DI;

    /// <summary>
    /// 数据服务实现类，通过 SqlSugar ORM 操作 SQLite 数据库，提供欢迎消息的存取功能。
    /// Data service implementation that uses SqlSugar ORM to operate on a SQLite database, providing welcome message storage and retrieval.
    /// </summary>
    public class DataService : IDataService, ISingleton
    {
        /// <summary>
        /// SqlSugar 数据库客户端实例。
        /// SqlSugar database client instance.
        /// </summary>
        private readonly SqlSugarClient _db;

        /// <summary>
        /// 构造函数：初始化 Message 表结构（CodeFirst），若表为空则插入一条默认欢迎消息。
        /// Constructor: initializes the Message table structure (CodeFirst); inserts a default welcome message if the table is empty.
        /// </summary>
        /// <param name="db">SqlSugar 数据库客户端实例 / The SqlSugar database client instance.</param>
        public DataService(SqlSugarClient db)
        {
            _db = db;
            _db.CodeFirst.InitTables<Message>();

            var any = _db.Queryable<Message>().Any();
            if (!any)
            {
                _db.Insertable(new Message
                {
                    Content = "欢迎使用简洁架构",
                    CreatedAt = DateTime.Now
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 获取最新的欢迎消息（按创建时间降序排列取第一条）。
        /// Retrieves the latest welcome message (ordered by creation time descending, returns the first one).
        /// </summary>
        /// <returns>最新的欢迎消息实体 / The latest welcome message entity.</returns>
        public Message GetWelcomeMessage()
        {
            return _db.Queryable<Message>()
                      .OrderBy(x => x.CreatedAt, OrderByType.Desc)
                      .First();
        }
    }
}
