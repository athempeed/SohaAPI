using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication10.Services
{
    public class BlobService 
    {        
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=sohadatastore;AccountKey=elYb0nUFF8twmpVOrgGasnDTNjSHnuMFQngqTMcIZodDh/h0gTENz4VDlvt+HqEZx0M4Q0LLNzBGIlyDkC2+Qg==;EndpointSuffix=core.windows.net";
        
        
        
        private async Task<CloudBlobContainer> GetContainerAsync()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "qbotokencontainer";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return cloudBlobContainer;
        }
        private async Task<CloudBlobContainer> GetContainerAsync(string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = containerName;
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return cloudBlobContainer;
        }

        public async Task UploadFileToBlobAsync(string strFileName, string strJSON)
        {
            try
            {
                var cloudBlobContainer = await GetContainerAsync();
                string fileName = strFileName;
                if (fileName != null && !string.IsNullOrWhiteSpace(strJSON))
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    await cloudBlockBlob.UploadTextAsync(strJSON);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task UploadFileToBlobAsync(string strFileName, string containerName, string strJSON)
        {
            try
            {
                var cloudBlobContainer = await GetContainerAsync(containerName);
                string fileName = strFileName;
                if (fileName != null && !string.IsNullOrWhiteSpace(strJSON))
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    await cloudBlockBlob.UploadTextAsync(strJSON);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> ReadFileToBlobAsync(string strFileName)
        {
            try
            {
                var cloudBlobContainer = await GetContainerAsync();
                string fileName = strFileName;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    if (await cloudBlockBlob.ExistsAsync())
                    {
                        return await cloudBlockBlob.DownloadTextAsync();
                    }

                    //return cloudBlockBlob.CopyState.Status.ToString();
                }
                return "";
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> ReadFileToBlobAsync(string strFileName, string containerName)
        {
            try
            {
                var cloudBlobContainer = await GetContainerAsync(containerName);
                string fileName = strFileName;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    if (await cloudBlockBlob.ExistsAsync())
                    {
                        return await cloudBlockBlob.DownloadTextAsync();
                    }

                    //return cloudBlockBlob.CopyState.Status.ToString();
                }
                return "";
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteBlobDataAsync(string fileUrl)
        {
            try
            {

                Uri uriObj = new Uri(fileUrl);
                string BlobName = Path.GetFileName(uriObj.LocalPath);

                var cloudBlobContainer = await GetContainerAsync();
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(BlobName);

                // delete blob from container        
                await blockBlob.DeleteAsync();
            }
            catch
            {
                throw;
            }
        }

    }
}
