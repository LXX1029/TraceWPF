namespace TraceWPF.Infrastructure.Services
{
    using System;
    using System.Linq;
    using SqlSugar;
    using TraceWPF.Domain.Interfaces;
    using TraceWPF.Domain.Models;
    using TraceWPF.DI;

    public class DataService : IDataService, ISingleton
    {
        private readonly SqlSugarClient _db;

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

        public Message GetWelcomeMessage()
        {
            return _db.Queryable<Message>()
                      .OrderBy(x => x.CreatedAt, OrderByType.Desc)
                      .First();
        }
    }
}
