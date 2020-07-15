#if !BESTHTTP_DISABLE_CACHING

using System;
using System.Collections.Generic;
using System.Threading;

//
// Version 1: Initial release
// Version 2: Filenames are generated from an index.
//

namespace BestHTTP.Caching
{
    using BestHTTP.Extensions;
    using BestHTTP.PlatformSupport.FileSystem;

    public sealed class UriComparer : IEqualityComparer<Uri>
    {
        public bool Equals(Uri x, Uri y)
        {
            return Uri.Compare(x, y, UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped, StringComparison.Ordinal) == 0;
        }

        public int GetHashCode(Uri uri)
        {
            return uri.ToString().GetHashCode();
        }
    }


    public static class HTTPCacheService
    {
        #region Properties & Fields

        /// <summary>
        /// Library file-format versioning support
        /// </summary>
        private const int LibraryVersion = 2;

        public static bool IsSupported
        {
            get
            {
                if (IsSupportCheckDone)
                    return isSupported;

                try
                {
                    // If DirectoryExists throws an exception we will set IsSupprted to false

                    HTTPManager.IOService.DirectoryExists(HTTPManager.GetRootCacheFolder());
                    isSupported = true;
                }
                catch
                {
                    isSupported = false;

                    HTTPManager.Logger.Warning("HTTPCacheService", "Cache Service Disabled!");
                }
                finally
                {
                    IsSupportCheckDone = true;
                }

                return isSupported;
            }
        }
        private static bool isSupported;
        private static bool IsSupportCheckDone;

        private static Dictionary<Uri, HTTPCacheFileInfo> library;

        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private static Dictionary<UInt64, HTTPCacheFileInfo> UsedIndexes = new Dictionary<ulong, HTTPCacheFileInfo>();

        internal static string CacheFolder { get; private set; }
        private static string LibraryPath { get; set; }

        private volatile static bool InClearThread;
        private volatile static bool InMaintainenceThread;

        public static bool IsDoingMaintainence { get { return InClearThread || InMaintainenceThread; } }

        /// <summary>
        /// Stores the index of the next stored entity. The entity's file name is generated from this index.
        /// </summary>
        private static UInt64 NextNameIDX;

        #endregion

        static HTTPCacheService()
        {
            NextNameIDX = 0x0001;
        }

        #region Common Functions

        internal static void CheckSetup()
        {
            if (!HTTPCacheService.IsSupported)
                return;

            try
            {
                SetupCacheFolder();
                LoadLibrary();
            }
            catch
            { }
        }

        internal static void SetupCacheFolder()
        {
            if (!HTTPCacheService.IsSupported)
                return;

            try
            {
                if (string.IsNullOrEmpty(CacheFolder) || string.IsNullOrEmpty(LibraryPath))
                {
                    CacheFolder = System.IO.Path.Combine(HTTPManager.GetRootCacheFolder(), "HTTPCache");
                    if (!HTTPManager.IOService.DirectoryExists(CacheFolder))
                        HTTPManager.IOService.DirectoryCreate(CacheFolder);

                    LibraryPath = System.IO.Path.Combine(HTTPManager.GetRootCacheFolder(), "Library");
                }
            }
            catch
            {
                isSupported = false;

                HTTPManager.Logger.Warning("HTTPCacheService", "Cache Service Disabled!");
            }
        }

        internal static UInt64 GetNameIdx()
        {
            UInt64 result = NextNameIDX;

            do
            {
                NextNameIDX = ++NextNameIDX % UInt64.MaxValue;
            } while (UsedIndexes.ContainsKey(NextNameIDX));

            return result;
        }

        internal static bool HasEntity(Uri uri)
        {
            if (!IsSupported)
                return false;

            CheckSetup();

            rwLock.EnterReadLock();
            try
            {
                return library.ContainsKey(uri);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static bool DeleteEntity(Uri uri, bool removeFromLibrary = true)
        {
            if (!IsSupported)
                return false;

            // 2019.05.10: Removed all locking except the one on the library.

            CheckSetup();

            rwLock.EnterUpgradeableReadLock();
            try
            {
                DeleteEntityImpl(uri, removeFromLibrary, true);

                return true;
            }
            finally
            {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        private static void DeleteEntityImpl(Uri uri, bool removeFromLibrary = true, bool useLocking = false)
        {
            HTTPCacheFileInfo info;
            bool inStats = library.TryGetValue(uri, out info);
            if (inStats)
                info.Delete();

            if (inStats && removeFromLibrary)
            {
                rwLock.EnterWriteLock();
                library.Remove(uri);
                UsedIndexes.Remove(info.MappedNameIDX);
                rwLock.ExitWriteLock();
            }
        }

        internal static bool IsCachedEntityExpiresInTheFuture(HTTPRequest request)
        {
            if (!IsSupported)
                return false;

            CheckSetup();

            HTTPCacheFileInfo info = null;
            rwLock.EnterReadLock();
            try
            {
                if (!library.TryGetValue(request.CurrentUri, out info))
                    return false;
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            return info.WillExpireInTheFuture();
        }

        /// <summary>
        /// Utility function to set the cache control headers according to the spec.: http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.3.4
        /// </summary>
        /// <param name="request"></param>
        internal static void SetHeaders(HTTPRequest request)
        {
            if (!IsSupported)
                return;

            CheckSetup();

            request.RemoveHeader("If-None-Match");
            request.RemoveHeader("If-Modified-Since");

            HTTPCacheFileInfo info = null;
            rwLock.EnterReadLock();
            try
            {
                if (!library.TryGetValue(request.CurrentUri, out info))
                    return;
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            info.SetUpRevalidationHeaders(request);
        }

        #endregion

        #region Get Functions

        internal static HTTPCacheFileInfo GetEntity(Uri uri)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            HTTPCacheFileInfo info = null;

            rwLock.EnterReadLock();
            try
            {
                library.TryGetValue(uri, out info);
            }
            finally
            {
                rwLock.ExitReadLock();
            }

            return info;
        }

        internal static HTTPResponse GetFullResponse(HTTPRequest request)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            HTTPCacheFileInfo info = null;
            rwLock.EnterReadLock();
            try
            {
                if (!library.TryGetValue(request.CurrentUri, out info))
                    return null;

                return info.ReadResponseTo(request);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        #endregion

        #region Storing

        /// <summary>
        /// Checks if the given response can be cached. http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.4
        /// </summary>
        /// <returns>Returns true if cacheable, false otherwise.</returns>
        internal static bool IsCacheble(Uri uri, HTTPMethods method, HTTPResponse response)
        {
            if (!IsSupported)
                return false;

            if (method != HTTPMethods.Get)
                return false;

            if (response == null)
                return false;

            // https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.12 - Cache Replacement
            // It MAY insert it into cache storage and MAY, if it meets all other requirements, use it to respond to any future requests that would previously have caused the old response to be returned.
            //if (response.StatusCode == 304)
            //    return false;

            if (response.StatusCode < 200 || response.StatusCode >= 400)
                return false;

            //http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.9.2
            var cacheControls = response.GetHeaderValues("cache-control");
            if (cacheControls != null)
            {
                if (cacheControls.Exists(headerValue =>
                {
                    string value = headerValue.ToLower();
                    if (value.StartsWith("max-age"))
                    {
                        string[] kvp = headerValue.FindOption("max-age");
                        if (kvp != null)
                        {
                            // Some cache proxies will return float values
                            double maxAge;
                            if (double.TryParse(kvp[1], out maxAge))
                            {
                                // A negative max-age value is a no cache
                                if (maxAge <= 0)
                                    return false;
                            }
                        }

                    }

                    return value.Contains("no-store") || value.Contains("no-cache");
                }))
                    return false;
            }

            var pragmas = response.GetHeaderValues("pragma");
            if (pragmas != null)
            {
                if (pragmas.Exists(headerValue =>
                {
                    string value = headerValue.ToLower();
                    return value.Contains("no-store") || value.Contains("no-cache");
                }))
                    return false;
            }

            // Responses with byte ranges not supported yet.
            var byteRanges = response.GetHeaderValues("content-range");
            if (byteRanges != null)
                return false;

            var etag = response.GetFirstHeaderValue("ETag");
            if (!string.IsNullOrEmpty(etag))
                return true;

            var expires = response.GetFirstHeaderValue("Expires").ToDateTime(DateTime.FromBinary(0));
            if (expires >= DateTime.UtcNow)
                return true;

            if (response.GetFirstHeaderValue("Last-Modified") != null)
                return true;

            return false;
        }

        internal static HTTPCacheFileInfo Store(Uri uri, HTTPMethods method, HTTPResponse response)
        {
            if (response == null || response.Data == null || response.Data.Length == 0)
                return null;

            if (!IsSupported)
                return null;

            CheckSetup();

            HTTPCacheFileInfo info = null;

            rwLock.EnterWriteLock();
            try
            {
                if (!library.TryGetValue(uri, out info))
                {
                    library.Add(uri, info = new HTTPCacheFileInfo(uri));
                    UsedIndexes.Add(info.MappedNameIDX, info);
                }

                try
                {
                    info.Store(response);
                    if (HTTPManager.Logger.Level == Logger.Loglevels.All)
                        HTTPManager.Logger.Verbose("HTTPCacheService", string.Format("{0} - Saved to cache", uri.ToString()));
                }
                catch
                {
                    // If something happens while we write out the response, than we will delete it because it might be in an invalid state.
                    DeleteEntityImpl(uri);

                    throw;
                }
            }
            finally
            {
                rwLock.ExitWriteLock();
            }

            return info;
        }

        internal static System.IO.Stream PrepareStreamed(Uri uri, HTTPResponse response)
        {
            if (!IsSupported)
                return null;

            CheckSetup();

            HTTPCacheFileInfo info;

            rwLock.EnterWriteLock();
            try
            {
                if (!library.TryGetValue(uri, out info))
                {
                    library.Add(uri, info = new HTTPCacheFileInfo(uri));
                    UsedIndexes.Add(info.MappedNameIDX, info);
                }
            }
            finally
            {
                rwLock.ExitWriteLock();
            }

            try
            {
                return info.GetSaveStream(response);
            }
            catch
            {
                // If something happens while we write out the response, than we will delete it because it might be in an invalid state.
                DeleteEntityImpl(uri, true, true);

                throw;
            }
        }

        #endregion

        #region Public Maintenance Functions

        /// <summary>
        /// Deletes all cache entity. Non blocking.
        /// <remarks>Call it only if there no requests currently processed, because cache entries can be deleted while a server sends back a 304 result, so there will be no data to read from the cache!</remarks>
        /// </summary>
        public static void BeginClear()
        {
            if (!IsSupported)
                return;

            if (InClearThread)
                return;
            InClearThread = true;

            SetupCacheFolder();

            PlatformSupport.Threading.ThreadedRunner.RunShortLiving(ClearImpl);
        }

        private static void ClearImpl()
        {
            if (!IsSupported)
                return;

            CheckSetup();

            rwLock.EnterWriteLock();
            try
            {
                // GetFiles will return a string array that contains the files in the folder with the full path
                string[] cacheEntries = HTTPManager.IOService.GetFiles(CacheFolder);

                for (int i = 0; i < cacheEntries.Length; ++i)
                {
                    // We need a try-catch block because between the Directory.GetFiles call and the File.Delete calls a maintenance job, or other file operations can delete any file from the cache folder.
                    // So while there might be some problem with any file, we don't want to abort the whole for loop
                    try
                    {
                        HTTPManager.IOService.FileDelete(cacheEntries[i]);
                    }
                    catch
                    { }
                }
            }
            finally
            {
                UsedIndexes.Clear();
                library.Clear();
                NextNameIDX = 0x0001;

                InClearThread = false;

                rwLock.ExitWriteLock();

                SaveLibrary();
            }
        }

        /// <summary>
        /// Deletes all expired cache entity.
        /// <remarks>Call it only if there no requests currently processed, because cache entries can be deleted while a server sends back a 304 result, so there will be no data to read from the cache!</remarks>
        /// </summary>
        public static void BeginMaintainence(HTTPCacheMaintananceParams maintananceParam)
        {
            if (maintananceParam == null)
                throw new ArgumentNullException("maintananceParams == null");

            if (!HTTPCacheService.IsSupported)
                return;

            if (InMaintainenceThread)
                return;

            InMaintainenceThread = true;

            SetupCacheFolder();

            PlatformSupport.Threading.ThreadedRunner.RunShortLiving(MaintananceImpl, maintananceParam);
        }

        private static void MaintananceImpl(HTTPCacheMaintananceParams maintananceParam)
        {
            CheckSetup();

            rwLock.EnterWriteLock();
            try
            {
                // Delete cache entries older than the given time.
                DateTime deleteOlderAccessed = DateTime.UtcNow - maintananceParam.DeleteOlder;
                List<HTTPCacheFileInfo> removedEntities = new List<HTTPCacheFileInfo>();
                foreach (var kvp in library)
                    if (kvp.Value.LastAccess < deleteOlderAccessed)
                    {
                        DeleteEntityImpl(kvp.Key, false, false);
                        removedEntities.Add(kvp.Value);
                    }

                for (int i = 0; i < removedEntities.Count; ++i)
                {
                    library.Remove(removedEntities[i].Uri);
                    UsedIndexes.Remove(removedEntities[i].MappedNameIDX);
                }
                removedEntities.Clear();

                ulong cacheSize = GetCacheSize();

                // This step will delete all entries starting with the oldest LastAccess property while the cache size greater then the MaxCacheSize in the given param.
                if (cacheSize > maintananceParam.MaxCacheSize)
                {
                    List<HTTPCacheFileInfo> fileInfos = new List<HTTPCacheFileInfo>(library.Count);

                    foreach (var kvp in library)
                        fileInfos.Add(kvp.Value);

                    fileInfos.Sort();

                    int idx = 0;
                    while (cacheSize >= maintananceParam.MaxCacheSize && idx < fileInfos.Count)
                    {
                        try
                        {
                            var fi = fileInfos[idx];
                            ulong length = (ulong)fi.BodyLength;

                            DeleteEntityImpl(fi.Uri);

                            cacheSize -= length;
                        }
                        catch
                        { }
                        finally
                        {
                            ++idx;
                        }
                    }
                }
            }
            finally
            {
                InMaintainenceThread = false;
                rwLock.ExitWriteLock();

                SaveLibrary();
            }
        }
        
        public static int GetCacheEntityCount()
        {
            if (!HTTPCacheService.IsSupported)
                return 0;

            CheckSetup();

            rwLock.EnterReadLock();
            try
            {
                return library.Count;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static ulong GetCacheSize()
        {
            ulong size = 0;

            if (!IsSupported)
                return size;

            CheckSetup();

            rwLock.EnterReadLock();
            try
            {
                foreach (var kvp in library)
                    if (kvp.Value.BodyLength > 0)
                        size += (ulong)kvp.Value.BodyLength;
                return size;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        #endregion

        #region Cache Library Management

        private static void LoadLibrary()
        {
            // Already loaded?
            if (library != null)
                return;

            if (!IsSupported)
                return;

            int version = 1;

            rwLock.EnterWriteLock();

            library = new Dictionary<Uri, HTTPCacheFileInfo>(new UriComparer());
            try
            {
                using (var fs = HTTPManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.Open))
                using (var br = new System.IO.BinaryReader(fs))
                {
                    version = br.ReadInt32();

                    if (version > 1)
                        NextNameIDX = br.ReadUInt64();

                    int statCount = br.ReadInt32();

                    for (int i = 0; i < statCount; ++i)
                    {
                        Uri uri = new Uri(br.ReadString());

                        var entity = new HTTPCacheFileInfo(uri, br, version);
                        if (entity.IsExists())
                        {
                            library.Add(uri, entity);

                            if (version > 1)
                                UsedIndexes.Add(entity.MappedNameIDX, entity);
                        }
                    }
                }
            }
            catch
            { }
            finally
            {
                rwLock.ExitWriteLock();
            }

            if (version == 1)
                BeginClear();
            else
                DeleteUnusedFiles();
        }

        internal static void SaveLibrary()
        {
            if (library == null)
                return;

            if (!IsSupported)
                return;

            rwLock.EnterReadLock();
            try
            {
                using (var fs = HTTPManager.IOService.CreateFileStream(LibraryPath, FileStreamModes.Create))
                using (var bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(LibraryVersion);
                    bw.Write(NextNameIDX);

                    bw.Write(library.Count);
                    foreach (var kvp in library)
                    {
                        bw.Write(kvp.Key.ToString());

                        kvp.Value.SaveTo(bw);
                    }
                }
            }
            catch
            { }
            finally
            {
                rwLock.ExitReadLock();
            }
        }


        internal static void SetBodyLength(Uri uri, int bodyLength)
        {
            if (!IsSupported)
                return;

            CheckSetup();

            rwLock.EnterUpgradeableReadLock();
            try
            {
                HTTPCacheFileInfo fileInfo;
                if (library.TryGetValue(uri, out fileInfo))
                    fileInfo.BodyLength = bodyLength;
                else
                {
                    rwLock.EnterWriteLock();
                    try
                    {
                        library.Add(uri, fileInfo = new HTTPCacheFileInfo(uri, DateTime.UtcNow, bodyLength));
                        UsedIndexes.Add(fileInfo.MappedNameIDX, fileInfo);
                    }
                    finally
                    {
                        rwLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Deletes all files from the cache folder that isn't in the Library.
        /// </summary>
        private static void DeleteUnusedFiles()
        {
            if (!IsSupported)
                return;

            CheckSetup();

            // GetFiles will return a string array that contains the files in the folder with the full path
            string[] cacheEntries = HTTPManager.IOService.GetFiles(CacheFolder);

            for (int i = 0; i < cacheEntries.Length; ++i)
            {
                // We need a try-catch block because between the Directory.GetFiles call and the File.Delete calls a maintenance job, or other file operations can delete any file from the cache folder.
                // So while there might be some problem with any file, we don't want to abort the whole for loop
                try
                {
                    string filename = System.IO.Path.GetFileName(cacheEntries[i]);
                    UInt64 idx = 0;
                    bool deleteFile = false;
                    if (UInt64.TryParse(filename, System.Globalization.NumberStyles.AllowHexSpecifier, null, out idx))
                    {
                        rwLock.EnterReadLock();
                        try
                        {
                            deleteFile = !UsedIndexes.ContainsKey(idx);
                        }
                        finally
                        {
                            rwLock.ExitReadLock();
                        }
                    }
                    else
                        deleteFile = true;

                    if (deleteFile)
                        HTTPManager.IOService.FileDelete(cacheEntries[i]);
                }
                catch
                { }
            }
        }

        #endregion
    }
}

#endif