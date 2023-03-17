
using System.Threading.Tasks;
using System.Threading;
using RestHTTP;
using SketchfabAPI;
using Utils.ProgressReporting;
using App.Infrastructure.Contexts;
using App.Core.Factories;
using App.UI;

namespace App.Services
{
    public class SketchfabModelDownloader
    {
        public async Task<Result<byte[]>> DownloadGlbAsync(string modelId, IProgressReporter<ProgressInfo> progressReporter, CancellationToken cancellationToken)
        {
            IContext mainContext = MainContext.Instance;
            Result<byte[]> result = new Result<byte[]>();
            UIConfig _uiConfig = mainContext.Get<AppConfig>().UIConfig;

            SketchfabAPIFactory sketchfabAPIFactory = mainContext.Get<SketchfabAPIFactory>();
            DownloadAPI downloadAPI = sketchfabAPIFactory.CreateAPI<DownloadAPI>();
            Result<ArchivesEntity> archivesResult = await downloadAPI.GetArchiveLinksAsync(modelId);

            ArchivesEntity archives = archivesResult.Entity;
            bool isSucceed = archives != null;

            if (isSucceed)
            {
                ProgressInfo progressInfo = new ProgressInfo();
                progressInfo.ProgressNormalizedValue = 0f;
                progressInfo.Message = _uiConfig.Texts["StartDownloading"];
                progressReporter.Report(progressInfo);

                result = await downloadAPI.DownloadArchiveAsync(archives.Glb.Url, 
                    (RequestProgressData requestProgressData) =>
                    {
                        progressInfo.ProgressNormalizedValue = requestProgressData.Progress;
                        float progressPercent = progressInfo.ProgressNormalizedValue * 100f;
                        progressInfo.Message = string.Format(_uiConfig.Texts["Downloading"], progressPercent);
                        progressReporter.Report(progressInfo);
                    }, 
                    cancellationToken);
            }
            else
            {
                result.Message = archivesResult.Message;
                result.StatusCode = archivesResult.StatusCode;
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
