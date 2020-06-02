using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public void Connect()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection(App.DatabasePath);
                _connection.CreateTable<MeasurementEntity>();
            }
        }

        public void Insert(List<Measurement> data)
        {
            if (data == null) return;

            if (_connection == null) throw new Exception("Connection closed");

            var me = new MeasurementEntity();
            me.Measurement = JsonConvert.SerializeObject(data);
            me.DateTime = DateTime.Now;

            _connection.Insert(me);
        }

        public MeasurementEntity Select()
        {
            if (_connection != null)
            {
                var query = _connection.Table<MeasurementEntity>();
                var entities = query.ToList();
                var me = entities.Count() > 0 ? entities.LastOrDefault() : null;
                return me;
            }

            throw new Exception("Connection closed");
        }

        public void Truncate()
        {
            if (_connection != null)
            {
                _connection.DropTable<MeasurementEntity>();
                _connection.CreateTable<MeasurementEntity>();
            }
        }

        public void Disconnect()
        {
            Dispose();
        }
    }
}