using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Totoro.WinUI.Media;

public partial class HttpRandomAccessStream : IRandomAccessStreamWithContentType
{
    private readonly HttpClient client;
    private IInputStream inputStream;
    private ulong size;
    private ulong requestedPosition;
    private string etagHeader;
    private string lastModifiedHeader;
    private readonly Uri requestedUri;

    // No public constructor, factory methods instead to handle async tasks.
    private HttpRandomAccessStream(HttpClient client, Uri uri)
    {
        this.client = client;
        requestedUri = uri;
        requestedPosition = 0;
    }

    static public IAsyncOperation<HttpRandomAccessStream> CreateAsync(HttpClient client, Uri uri)
    {
        HttpRandomAccessStream randomStream = new HttpRandomAccessStream(client, uri);

        return AsyncInfo.Run(async (cancellationToken) =>
        {
            await randomStream.SendRequesAsync().ConfigureAwait(false);
            return randomStream;
        });
    }

    private async Task SendRequesAsync()
    {
        Debug.Assert(inputStream == null);

        HttpRequestMessage? request = null;
        request = new HttpRequestMessage(HttpMethod.Get, requestedUri);

        request.Headers.Add("Range", string.Format("bytes={0}-", requestedPosition));

        if (!string.IsNullOrEmpty(etagHeader))
        {
            request.Headers.Add("If-Match", etagHeader);
        }

        if (!string.IsNullOrEmpty(lastModifiedHeader))
        {
            request.Headers.Add("If-Unmodified-Since", lastModifiedHeader);
        }

        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

        if (response.Content.Headers.ContentType != null)
        {
            ContentType = response.Content.Headers.ContentType.MediaType;
        }

        size = (ulong?)response.Content.Headers.ContentLength ?? 0;

        if (response.StatusCode is not System.Net.HttpStatusCode.PartialContent && requestedPosition != 0)
        {
            throw new Exception("HTTP server did not reply with a '206 Partial Content' status.");
        }

        if (response.Headers.AcceptRanges is null)
        {
            //throw new Exception(string.Format(
            //    "HTTP server does not support range requests: {0}",
            //    "http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.5"));
        }

        if (string.IsNullOrEmpty(etagHeader) && response.Headers.ETag is not null)
        {
            etagHeader = response.Headers.ETag.Tag;
        }

        if (string.IsNullOrEmpty(lastModifiedHeader) && response.Content.Headers.LastModified.HasValue)
        {
            lastModifiedHeader = response.Content.Headers.LastModified.ToString();
        }
        if (response.Content.Headers.ContentType is not null)
        {
            contentType = response.Content.Headers.ContentType.MediaType;
        }

        inputStream = (await response!.Content!.ReadAsStreamAsync()).AsInputStream();
    }

    private string contentType = string.Empty;

    public string ContentType
    {
        get { return contentType; }
        private set { contentType = value; }
    }

    public bool CanRead
    {
        get
        {
            return true;
        }
    }

    public bool CanWrite
    {
        get
        {
            return false;
        }
    }

    public IRandomAccessStream CloneStream()
    {
        // If there is only one MediaPlayerElement using the stream, it is safe to return itself.
        return this;
    }

    public IInputStream GetInputStreamAt(ulong position)
    {
        throw new NotImplementedException();
    }

    public IOutputStream GetOutputStreamAt(ulong position)
    {
        throw new NotImplementedException();
    }

    public ulong Position
    {
        get
        {
            return requestedPosition;
        }
    }

    public void Seek(ulong position)
    {
        if (requestedPosition != position)
        {
            if (inputStream != null)
            {
                inputStream.Dispose();
                inputStream = null;
            }
            Debug.WriteLine("Seek: {0:N0} -> {1:N0}", requestedPosition, position);
            requestedPosition = position;
        }
    }

    public ulong Size
    {
        get
        {
            return size;
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public void Dispose()
    {
        if (inputStream != null)
        {
            inputStream.Dispose();
            inputStream = null;
        }
    }

    public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
    {
        return AsyncInfo.Run<IBuffer, uint>(async (cancellationToken, progress) =>
        {
            progress.Report(0);

            try
            {
                if (inputStream == null)
                {
                    await SendRequesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }

            IBuffer result = await inputStream.ReadAsync(buffer, count, options).AsTask(cancellationToken, progress).ConfigureAwait(false);

            // Move position forward.
            requestedPosition += result.Length;
            Debug.WriteLine("requestedPosition = {0:N0}", requestedPosition);

            return result;
        });
    }

    public IAsyncOperation<bool> FlushAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
    {
        throw new NotImplementedException();
    }
}
