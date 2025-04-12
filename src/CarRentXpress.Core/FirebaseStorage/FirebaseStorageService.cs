using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;
using System.Net.Http; // For HttpMethod.Get
using System.Threading.Tasks;
using CarRentXpress.Core.Repositories;
using Google.Cloud.Storage.V1;
using static Google.Cloud.Storage.V1.UrlSigner;
using Microsoft.Extensions.Options;

public class FirebaseStorageService : IFileUploadService
{
    private readonly string _firebaseBucketUrl = "carrentxpress.firebasestorage.app";
    private readonly string _jsonCredentialPath = "C:\\Users\\Martin\\Git\\CarRentXpress\\src\\CarRentXpress.Data\\carrentxpress-firebase-adminsdk-fbsvc-655e8a78b0.json";
    private FirebaseApp _firebaseApp;
    private StorageClient _storageClient;

    public FirebaseStorageService()
    {
        InitializeFirebaseStorage();
    }

    private void InitializeFirebaseStorage()
    {
        var credential = GoogleCredential.FromFile(_jsonCredentialPath);

        // Check if a default FirebaseApp exists; if not, create one
        if (FirebaseApp.DefaultInstance is null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = credential
            });
        }

        // Create a StorageClient instance with the credential.
        _storageClient = StorageClient.Create(credential);
    }

    // Upload a file to Firebase Storage and return a signed URL.
    public async Task<string> UploadFileAsync(Stream fileStream, string destinationPath)
    {
        if (fileStream == null)
        {
            throw new ArgumentNullException(nameof(fileStream), "File stream cannot be null");
        }
    
        var uploadedObject = await _storageClient.UploadObjectAsync(
            bucket: _firebaseBucketUrl,
            objectName: destinationPath,
            contentType: null, // optional
            source: fileStream);
        
        // Code to generate a signed URL (if needed)
        var urlSigner = UrlSigner.FromServiceAccountPath(_jsonCredentialPath);
        TimeSpan urlDuration = TimeSpan.FromDays(7);

        string signedUrl = urlSigner.Sign(
            bucket: _firebaseBucketUrl,
            objectName: destinationPath,
            duration: urlDuration,
            httpMethod: HttpMethod.Get);

        return signedUrl;
    }

}
