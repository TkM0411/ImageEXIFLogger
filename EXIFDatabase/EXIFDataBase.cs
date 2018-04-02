using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EXIFDatabase
{
    public class EXIFDatabaseController : IDisposable
    {
        #region Fields
        SqlConnection _connection;
        SqlCommand _command;
        private const string ConnectionStringFormat = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"{0}\";Integrated Security=True";
        #endregion

        #region Properties
        internal SqlCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        internal SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
        #endregion

        #region Constructor
        internal EXIFDatabaseController(string dbFileName = "")
        {
            if (string.IsNullOrEmpty(dbFileName))
                dbFileName = "..\\Build\\ImageEXIFMaster.mdf";
            Connection = new SqlConnection(string.Format(ConnectionStringFormat, dbFileName));
            Connection.Open();
            Command = new SqlCommand();
            Command.Connection = _connection;
            Command.CommandType = CommandType.StoredProcedure;
        }
        #endregion

        #region Private Methods
        private SqlParameter CreateParameter<T>(string parameterName, SqlDbType parameterType, T value, int length = 0)
        {
            SqlParameter param = new SqlParameter();
            param.SqlDbType = parameterType;
            param.ParameterName = !parameterName.StartsWith("@") ? string.Format("@{0}", parameterName) : parameterName;
            param.SqlValue = value;
            param.Direction = ParameterDirection.Input;
            if (length != 0)
                param.Size = length;
            return param;
        }

        private void InitializeCommand(string procedureName = "")
        {
            if (Command.Parameters.Count > 0)
                Command.Parameters.Clear();
            if (!string.IsNullOrEmpty(procedureName))
                Command.CommandText = procedureName;
        }

        private SqlParameter CreateNullParameter(string parameterName, SqlDbType parameterType)
        {
            SqlParameter nullParameter = new SqlParameter();
            nullParameter.ParameterName = parameterName.StartsWith("@") ? parameterName : string.Format("@{0}", parameterName);
            nullParameter.SqlDbType = parameterType;
            nullParameter.Direction = ParameterDirection.Input;
            nullParameter.SqlValue = DBNull.Value;
            return nullParameter;
        }
        #endregion

        #region Public Methods
        #region ConfigItems
        public int AddOrUpdateConfigItem(KeyValuePair<string, string> configItem)
        {
            int status = 0;
            try
            {
                InitializeCommand("spAddOrUpdateConfigItem");
                Command.Parameters.Add(CreateParameter<string>("@ConfigItemKey", SqlDbType.NVarChar, configItem.Key.Trim(), 20));
                Command.Parameters.Add(CreateParameter<string>("@ConfigItemValue", SqlDbType.NVarChar, configItem.Value.Trim(), 100));
                status = Command.ExecuteNonQuery() > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                status = -99;
                throw;
            }
            return status;
        }

        public int AddOrUpdateConfigItem(Dictionary<string, string> configItems)
        {
            int status = 0;
            try
            {
                if (configItems != null && configItems.Count > 0)
                {
                    foreach (KeyValuePair<string, string> config in configItems)
                    {
                        status = AddOrUpdateConfigItem(config);
                        if (status != 0)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                status = -99;
                throw;
            }
            return status;
        }

        public KeyValuePair<string, string>? GetConfigItem(string configItemKey)
        {
            KeyValuePair<string, string>? configItem = null;
            try
            {
                InitializeCommand("spGetConfigItem");
                Command.Parameters.Add(CreateParameter<string>("@ConfigItemKey", SqlDbType.NVarChar, configItemKey.Trim(), 20));
                SqlDataReader reader = Command.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        configItem = new KeyValuePair<string, string>(Convert.ToString(reader["ConfigItemKey"]).Trim(), Convert.ToString(reader["ConfigItemValue"]).Trim());
                    }
                    reader.Close();
                }
            }
            catch (Exception)
            {
                configItem = null;
                throw;
            }
            return configItem;
        }

        public Dictionary<string, string> GetAllConfigItems()
        {
            Dictionary<string, string> configItems = null;
            try
            {
                InitializeCommand("spGetAllConfigItems");
                SqlDataReader reader = Command.ExecuteReader();
                if (reader != null && reader.HasRows)
                {
                    configItems = new Dictionary<string, string>();
                    while (reader.Read())
                    {
                        string key = Convert.ToString(reader["ConfigItemKey"]).Trim();
                        string value = Convert.ToString(reader["ConfigItemValue"]).Trim();
                        if (!configItems.ContainsKey(key))
                        {
                            configItems.Add(key, value);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                configItems = null;
                throw;
            }
            return configItems;
        }
        #endregion
        #region FolderSettings
        public int AddFolder(ulong folderID, string folderPath, bool? isWatched)
        {
            int status = 0;
            try
            {
                InitializeCommand("spAddFolder");
                Command.Parameters.Add(CreateParameter<ulong>("@FolderID", SqlDbType.BigInt, folderID));
                Command.Parameters.Add(CreateParameter<string>("@FolderPath", SqlDbType.NVarChar, folderPath.Trim(), 512));
                if (isWatched.HasValue)
                    Command.Parameters.Add(CreateParameter<bool>("@IsWatched", SqlDbType.Bit, isWatched.Value));
                else
                    Command.Parameters.Add(CreateNullParameter("@IsWatched", SqlDbType.Bit));
                status = Command.ExecuteNonQuery() > 0 ? 0 : -1;
            }
            catch (Exception)
            {
                status = -99;
                throw;
            }
            return status;
        }

        public SqlDataReader GetAllFolders()
        {
            SqlDataReader reader = null;
            try
            {
                InitializeCommand("spGetWatchedFolders");
                reader = Command.ExecuteReader();
            }
            catch (Exception)
            {
                reader = null;
                throw;
            }
            return reader;
        }

        public SqlDataReader GetAllFoldersFiltered(bool isWatched, ulong? parentFolderID = null)
        {
            SqlDataReader reader = null;
            try
            {
                InitializeCommand("spGetWatchedFolders");
                Command.Parameters.Add(CreateParameter<bool>("@IsWatched", SqlDbType.Bit, isWatched));
                if (parentFolderID.HasValue)
                    Command.Parameters.Add(CreateParameter<ulong>("@ParentFolderID", SqlDbType.BigInt, parentFolderID.Value));
                else
                    Command.Parameters.Add(CreateNullParameter("@ParentFolderID", SqlDbType.BigInt));
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                throw;
            }
            return reader;
        }

        public SqlDataReader GetAllSubfolders(string parentFolderPath)
        {
            SqlDataReader reader = null;
            try
            {
                InitializeCommand("spGetAllSubFolders");
                Command.Parameters.Add(CreateParameter<string>("@FolderPath", SqlDbType.NVarChar, parentFolderPath.Trim(), 512));
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                throw;
            }
            return reader;
        }

        public int AddOrUpdateFolderSettings(ulong folderID, string folderPath, bool isWatched, ulong parentFolderID)
        {
            int status = 0;
            try
            {
                InitializeCommand("spAddOrUpdateFolderSettings");
                Command.Parameters.Add(CreateParameter<ulong>("@FolderID", SqlDbType.BigInt, folderID));
                Command.Parameters.Add(CreateParameter<string>("@FolderPath", SqlDbType.NVarChar, folderPath.Trim(), 512));
                Command.Parameters.Add(CreateParameter<bool>("@IsWatched", SqlDbType.Bit, isWatched));
                Command.Parameters.Add(CreateParameter<ulong>("@ParentFolderID", SqlDbType.BigInt, parentFolderID));
                status = Command.ExecuteNonQuery() > 0 ? 0 : -1;
            }
            catch (Exception)
            {
                status = -99;
                throw;
            }
            return status;
        }
        #endregion
        #region CameraInformation
        public int AddCameraInformation(string cameraMake, string cameraModel)
        {
            int status = 0;
            try
            {
                InitializeCommand("spAddCameraInformation");
                Command.Parameters.Add(CreateParameter<string>("@CameraMake", SqlDbType.NVarChar, cameraMake, 50));
                Command.Parameters.Add(CreateParameter<string>("@CameraModel", SqlDbType.NVarChar, cameraModel, 50));
                status = Command.ExecuteNonQuery() > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                status = -99;
                throw;
            }
            return status;
        }

        public SqlDataReader GetCameraInformation(string cameraMake = "")
        {
            SqlDataReader reader = null;
            try
            {
                if (string.IsNullOrEmpty(cameraMake))
                    InitializeCommand("spGetAllCameraInformation");
                else
                {
                    InitializeCommand("spGetCameraInformationFromMake");
                    Command.Parameters.Add(CreateParameter<string>("@CameraMake", SqlDbType.NVarChar, cameraMake, 50));
                }
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                throw;
            }
            return reader;
        }
        #endregion
        #region EXIFInformation
        public int AddEXIFData(ulong imageID, string folderPath, string cameraMake, string cameraModel, string aperture, string shutterSpeed, ushort ISO, decimal focalLength, byte orientation, byte meteringMode, sbyte exposureBias, byte exposureMode, ushort pixelXDimension, ushort pixelYDimension, DateTime createdDateTime, DateTime modifiedDateTime)
        {
            int status = 0;
            try
            {
                InitializeCommand("spAddEXIFData");
                Command.Parameters.Add(CreateParameter<ulong>("@ImageID", SqlDbType.BigInt, imageID));
                Command.Parameters.Add(CreateParameter<string>("@FolderPath", SqlDbType.NVarChar, folderPath.Trim(), 512));
                Command.Parameters.Add(CreateParameter<string>("@CameraMake", SqlDbType.NVarChar, cameraMake.Trim(), 50));
                Command.Parameters.Add(CreateParameter<string>("@CameraModel", SqlDbType.NVarChar, cameraModel.Trim(), 50));
                Command.Parameters.Add(CreateParameter<string>("@Aperture", SqlDbType.NVarChar, aperture.Trim(), 6));
                Command.Parameters.Add(CreateParameter<string>("@ShutterSpeed", SqlDbType.NVarChar, shutterSpeed.Trim(), 10));
                Command.Parameters.Add(CreateParameter<ushort>("@ISO", SqlDbType.SmallInt, ISO));
                Command.Parameters.Add(CreateParameter<decimal>("@FocalLength", SqlDbType.Decimal, Decimal.Round(focalLength, 1)));
                Command.Parameters.Add(CreateParameter<byte>("@Orientation", SqlDbType.TinyInt, orientation));
                Command.Parameters.Add(CreateParameter<byte>("@MeteringMode", SqlDbType.TinyInt, meteringMode));
                Command.Parameters.Add(CreateParameter<sbyte>("@ExposureBias", SqlDbType.TinyInt, exposureBias));
                Command.Parameters.Add(CreateParameter<byte>("@ExposureMode", SqlDbType.TinyInt, exposureMode));
                Command.Parameters.Add(CreateParameter<ushort>("@PixelXDimension", SqlDbType.SmallInt, pixelXDimension));
                Command.Parameters.Add(CreateParameter<ushort>("@PixelYDimension", SqlDbType.SmallInt, pixelYDimension));
                Command.Parameters.Add(CreateParameter<DateTime>("@CreatedDateTime", SqlDbType.DateTime, createdDateTime));
                Command.Parameters.Add(CreateParameter<DateTime>("@ModifiedDateTime", SqlDbType.DateTime, modifiedDateTime));
                status = Command.ExecuteNonQuery() > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                status = -99;
                throw;
            }
            return status;
        }

        public SqlDataReader GetEXIFInformationForImage(ulong imageID)
        {
            SqlDataReader reader = null;
            try
            {
                InitializeCommand("spGetEXIFInformationForImage");
                Command.Parameters.Add(CreateParameter<ulong>("@ImageID", SqlDbType.BigInt, imageID));
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                throw;
            }
            return reader;
        }

        public SqlDataReader GetAllEXIFInformationForFolder(ulong folderID)
        {
            SqlDataReader reader = null;
            try
            {
                InitializeCommand("spGetAllEXIFInformationForFolder");
                Command.Parameters.Add(CreateParameter<ulong>("@FolderID", SqlDbType.BigInt, folderID));
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                throw;
            }
            return reader;
        }
        #endregion
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (_connection.State == ConnectionState.Broken || _connection.State == ConnectionState.Open)
                _connection.Close();
            _command.Dispose();
            _connection.Dispose();
        }
        #endregion
    }
}