using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core;
using Lxs.Core.Data;
using Lxs.Core.Domain.Messages;
using Lxs.Data;

namespace Lxs.Services.Messages
{
    public class QueuedEmailService : IQueuedEmailService
    {
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly IDbContext _dbContext;
        private static bool UseStoredProceduresIfSupported = true;

        public QueuedEmailService(IDbContext dbContext, IRepository<QueuedEmail> queuedEmailRepository)
        {
            _dbContext = dbContext;
            _queuedEmailRepository = queuedEmailRepository;
        }

        public void InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Insert(queuedEmail);
        }

        public void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Update(queuedEmail);
        }

        public void DeleteQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Delete(queuedEmail);
        }

        public QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            return _queuedEmailRepository.GetById(queuedEmailId);
        }

        public IList<QueuedEmail> GetQueuedEmailsByIds(int[] queuedEmailIds)
        {
            if (queuedEmailIds == null || queuedEmailIds.Length == 0)
                return new List<QueuedEmail>();

            var query = from qe in _queuedEmailRepository.Table
                        where queuedEmailIds.Contains(qe.Id)
                        select qe;
            var queuedEmails = query.ToList();
            //sort by passed identifiers
            var sortedQueuedEmails = new List<QueuedEmail>();
            foreach (int id in queuedEmailIds)
            {
                var queuedEmail = queuedEmails.Find(x => x.Id == id);
                if (queuedEmail != null)
                    sortedQueuedEmails.Add(queuedEmail);
            }
            return sortedQueuedEmails;
        }

        public IPagedList<QueuedEmail> SearchEmails(string fromEmail, string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, int maxSendTries, bool loadNewest, int pageIndex, int pageSize)
        {



            fromEmail = (fromEmail ?? String.Empty).Trim();
            toEmail = (toEmail ?? String.Empty).Trim();

            var query = _queuedEmailRepository.Table;
            if (!String.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!String.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = query.OrderByDescending(qe => qe.Priority);
            query = loadNewest ?
                ((IOrderedQueryable<QueuedEmail>)query).ThenByDescending(qe => qe.CreatedOnUtc) :
                ((IOrderedQueryable<QueuedEmail>)query).ThenBy(qe => qe.CreatedOnUtc);

            var queuedEmails = new PagedList<QueuedEmail>(query, pageIndex, pageSize);
            return queuedEmails;
        }

        public void DeleteAllEmails()
        {
            if (UseStoredProceduresIfSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                //TODO: do not hard-code the table name
                _dbContext.ExecuteSqlCommand("TRUNCATE TABLE [QueuedEmail]");
            }
            else
            {
                var queuedEmails = _queuedEmailRepository.Table.ToList();
                foreach (var qe in queuedEmails)
                    _queuedEmailRepository.Delete(qe);
            }
        }
    }
}
