using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SQLite;

namespace airmonitor.Models
{
    public class DatabaseHelper : IDisposable
    {
        private static SQLiteConnection _connection;

        public DatabaseHelper() 
        {
            Connect();
        }

        public void Connect()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection(App.DatabasePath);
                _connection.CreateTable<MeasurementEntity>();
            }
        }

        public async System.Threading.Tasks.Task InsertAsync(IEnumerable<Measurement> data)
        {
            if (_connection == null)
            {
                MeasurementEntity me = new MeasurementEntity();
                me.Measurement = data;
                me.DateTime = DateTime.Now;

                _connection.Insert(me);
            }
            else
            {
                throw new Exception("Connection closed");
            }
        }

        public MeasurementEntity Select()
        {
            if (_connection == null)
            {
                TableQuery<MeasurementEntity> query = _connection.Table<MeasurementEntity>();

                MeasurementEntity me = query.ToList().Last();
                Debug.Write(me.DateTime.ToString());
                return me ?? null;
            }
            else
            {
                throw new Exception("Connection closed");
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
