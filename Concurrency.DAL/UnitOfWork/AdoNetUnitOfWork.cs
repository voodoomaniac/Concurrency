using System;
using System.Data;

namespace Concurrency.DAL.UnitOfWork
{
    public class AdoNetUnitOfWork : IDisposable
    {
        private IDbConnection _connection;
        private readonly bool _ownsConnection;
        private IDbTransaction _transaction;

        public AdoNetUnitOfWork(IDbConnection connection, bool ownsConnection)
        {
            this._connection = connection;
            this._ownsConnection = ownsConnection;
            this._transaction = connection.BeginTransaction();
        }

        public IDbCommand CreateCommand()
        {
            var command = this._connection.CreateCommand();
            command.Transaction = this._transaction;
            return command;
        }

        public void SaveChanges()
        {
            if (this._transaction == null)
                throw new InvalidOperationException("Transaction have already been commited. Check your transaction handling.");

            this._transaction.Commit();
            this._transaction = null;
        }

        public void Dispose()
        {
            if (this._transaction != null)
            {
                this._transaction.Rollback();
                this._transaction = null;
            }

            if (this._connection != null && this._ownsConnection)
            {
                this._connection.Close();
                this._connection = null;
            }
        }
    }
}
