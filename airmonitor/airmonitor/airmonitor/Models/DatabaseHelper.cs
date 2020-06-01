using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SQLite;

namespace airmonitor.Models
{
    public class DatabaseHelper : IDisposable
    {
        private static SQLiteConnection _connection = null;

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

        public void Insert(List<Measurement> data)
        {
            if (_connection != null)
            {
                MeasurementEntity me = new MeasurementEntity();
                me.Measurement = JsonConvert.SerializeObject(data);
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
            if (_connection != null)
            {
                TableQuery<MeasurementEntity> query = _connection.Table<MeasurementEntity>();
                List<MeasurementEntity> entities = query.ToList();
                MeasurementEntity me = entities.Count() > 0 ? entities.LastOrDefault() : null;
                return me;
            }
            else
            {
                throw new Exception("Connection closed");
            }
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
