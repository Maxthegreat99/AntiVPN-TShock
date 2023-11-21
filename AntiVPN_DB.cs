using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System;
using TShockAPI.DB;
using TShockAPI;
using System.Collections.Generic;
using System.Collections;

namespace AntiVPN_TShock.Data
{

    public class IPBan
    {
        /// <summary>
        /// ID of the ipban.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// stores the target's IPs.
        /// </summary>
        public string Identifiers { get; set; }

        /// <summary>
        /// name of the target being ipbanned.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// the name of the mod who made this ipban.
        /// </summary>
        public string ResponsibleName { get; set; }

        /// <summary>
        /// Reason of the ipban.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The expiration moment of this ipban.
        /// </summary>
        public DateTime Expiration { get; set; }



    }
    public static class IPBans
    {
        private static IDbConnection db;

        /// <summary>
        /// Initializes the DB connection.
        /// </summary>
        public static void Initialize()
        {
            switch (TShock.Config.Settings.StorageType.ToLower())
            {
                case "mysql":
                    var dbHost = TShock.Config.Settings.MySqlHost.Split(':');

                    db = new MySqlConnection($"Server={dbHost[0]};" +
                                                $"Port={(dbHost.Length == 1 ? "3306" : dbHost[1])};" +
                                                $"Database={TShock.Config.Settings.MySqlDbName};" +
                                                $"Uid={TShock.Config.Settings.MySqlUsername};" +
                                                $"Pwd={TShock.Config.Settings.MySqlPassword};");

                    break;

                case "sqlite":
                    db = new SqliteConnection($"Data Source={Path.Combine(TShock.SavePath, AntiVPN_TShock.pluginFolder, "AntiVPN.sqlite")}");
                    break;

                default:
                    throw new ArgumentException("Invalid storage type in config.json!");
            }

            SqlTableCreator creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());

            creator.EnsureTableStructure(new SqlTable("ipbans",
                new SqlColumn("id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
                new SqlColumn("targetname", MySqlDbType.Text),
                new SqlColumn("usernamemod", MySqlDbType.Text),
                new SqlColumn("identifiers", MySqlDbType.Text),
                new SqlColumn("expiration", MySqlDbType.Text),
                new SqlColumn("reason", MySqlDbType.Text)));

            creator.EnsureTableStructure(new SqlTable("trustedips",
                new SqlColumn("ip", MySqlDbType.Text),
                new SqlColumn("timeadded", MySqlDbType.Text)));


        }

        public static void InsertTrustedIps(string ip, DateTime time)
        {
            string query = $"INSERT INTO trustedips (ip, timeadded) VALUES ('{ip}', '{time.ToString()}');";
            db.Query(query);
        }

        public static void RemoveTrustedIp(string ip)
        {
            string query = $"DELETE FROM trustedips WHERE ip = {ip}";
            db.Query(query);
        }

        public static IEnumerable<string> GetAllTrustedIps()
        {
            string query = $"SELECT * FROM trustedips;";
            using (var result = db.QueryReader(query))
            {
                while (result.Read())
                {
                    string ip = result.Get<string>("ip");

                    yield return ip;
                }
            }
        }

        public static DateTime GetTimeIpWasAdded(string ip)
        {
            string query = $"SELECT timeadded FROM trustedips WHERE ip = {ip};";
            using (var result = db.QueryReader(query))
            {
                if (result.Read())
                {
                    var dateTime = DateTime.Parse(result.Get<string>("timeadded"));
                    return dateTime;
                }
            }
            return new DateTime();
        }

        public static void Insert(string targetName, string usernamemod, string identifiers, DateTime expiration, string reason)
        {
            string query = $"INSERT INTO ipbans (targetname, usernamemod, identifiers, expiration, reason) VALUES ('{targetName}', '{usernamemod}', '{identifiers}', '{(expiration.ToString())}', '{reason}');";
            db.Query(query);
        }

        public static void Remove(int id)
        {
            string query = $"DELETE FROM ipbans WHERE id = {id}";
            db.Query(query);
        }

        public static IPBan Get(int id)
        {
            string query = $"SELECT * FROM ipbans WHERE id = {id};";

            using (var result = db.QueryReader(query))
            {
                if (result.Read())
                {
                    var ipban = new IPBan()
                    {
                        ID = result.Get<int>("id"),
                        TargetName = result.Get<string>("targetname"),
                        ResponsibleName = result.Get<string>("usernamemod"),
                        Identifiers = result.Get<string>("identifiers"),
                        Expiration = DateTime.Parse(result.Get<string>("expiration")),
                        Reason = result.Get<string>("reason")
                    };
                    return ipban;
                }
            }
            return null;
        }

        public static IEnumerable<IPBan> GetAll()
        {
            string query = $"SELECT * FROM ipbans;";
            using (var result = db.QueryReader(query))
            {
                while (result.Read())
                {
                    var ipban = new IPBan()
                    {
                        ID = result.Get<int>("id"),
                        TargetName = result.Get<string>("targetname"),
                        ResponsibleName = result.Get<string>("usernamemod"),
                        Identifiers = result.Get<string>("identifiers"),
                        Expiration = DateTime.Parse(result.Get<string>("expiration")),
                        Reason = result.Get<string>("reason")
                    };

                    yield return ipban;
                }
            }
        }

        public static void Update(int id, string targetName, string usernamemod, string identifiers, DateTime expiration, string reason)
        {
            string query = $"UPDATE ipbans SET targetname = '{targetName}', usernamemod = '{usernamemod}', identifiers = '{identifiers}', expiration = '{expiration.ToString()}', reason = '{reason}' WHERE id = {id};";
            db.Query(query);
        }
    }
    
}
