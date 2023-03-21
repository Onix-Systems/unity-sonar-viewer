
using System;
using UnityEngine;
using Newtonsoft.Json;
using SketchfabAPI;
using SketchfabAPI.Entities;
using App.Services.ModelARViewing;
using SketchfabAPI.DTOs;

namespace App.Services.AppDataStoring
{
    public class AppData
    {
        public static AppData Default { get; } = new AppData()
        {
            LastSearchParameters = new SearchDTO() 
            {
                ArchivesMaxVertexCount = 500000,
                ArchivesTextureMaxResolution = 4096
            }
        };

        [JsonIgnore]
        public Model LoadedModelObject { get; set; }

        [JsonIgnore]
        public ModelEntity SelectedModel { get; set; }

        [JsonIgnore]
        public SearchResultsEntity LastSearchResults { get; set; }

        [JsonIgnore]
        public bool IsDirty { get; set; }

        public AccessTokenEntity SketchfabAccessToken 
        {
            get => _sketchfabAccessToken;
            
            set 
            { 
                _sketchfabAccessToken = value; 
                IsDirty = true;
            }
        }


        public SearchDTO LastSearchParameters
        {
            get => _lastSearchParameters;
            
            set 
            { 
                _lastSearchParameters = value; 
                IsDirty = true; 
            } 
        }

        public CredentialsDTO LastAuthCredentials
        {
            get => _lastAuthCredentials;

            set
            {
                _lastAuthCredentials = value;
                IsDirty = true;
            }
        }

        private AccessTokenEntity _sketchfabAccessToken;
        private SearchDTO _lastSearchParameters;
        private CredentialsDTO _lastAuthCredentials;

        public static implicit operator AppData(string json)
        {
            AppData appData;

            try
            {
                appData = JsonConvert.DeserializeObject<AppData>(json);
            }
            catch
            {
                appData = AppData.Default;  
            }

            return appData;
        }

        public static implicit operator string(AppData appData)
        {
            return appData.ToString();
        }

        public override string ToString()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                return json;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
            }

            return string.Empty;
        }
    }
}