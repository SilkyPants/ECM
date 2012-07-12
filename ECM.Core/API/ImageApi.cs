using System;
using System.Net;
using System.Drawing;
using System.IO;
using Gdk;

namespace ECM.API
{
    public static class ImageApi
    {
        const string IMAGE_API_URL = "http://image.eveonline.com/";
        const string IMAGE_CACHE_DIR = "Resources/Images/";
        static bool m_IsRetrieving = false;

        public static bool IsRetrieving
        {
            get { return m_IsRetrieving; }
        }

        public enum ImageRequestType
        {
            Alliance,
            Character,
            Corporation,
            Item
        }

        public enum ImageRequestSize
        {
            Size30x30,
            Size32x32,
            Size64x64,
            Size128x128,
            Size200x200,
            Size256x256,
            Size512x512,
            Size1024x1024,
        }
		
		#region .Net Image Helpers
        public static Bitmap GetCharacterPortraitNET(long charID, ImageRequestSize size)
		{
			return new Bitmap(GetCharacterPortrait(charID, size));
		}
		
        public static Bitmap GetCorporationLogoNET(long corpID, ImageRequestSize size)
		{
			return new Bitmap(GetCorporationLogo(corpID, size));
		}
			
        public static Bitmap GetAllianceLogoNET(long allianceID, ImageRequestSize size)
		{
			return new Bitmap(GetAllianceLogo(allianceID, size));
		}
		
        public static Bitmap GetItemImageNET(long typeID, ImageRequestSize size)
		{
			return new Bitmap(GetItemImage(typeID, size));
		}
		
        public static Bitmap GetItemRenderNET(long typeID, ImageRequestSize size)
		{
			return new Bitmap(GetItemRender(typeID, size));
		}
		#endregion
		
		#region Gtk# Image Helpers
        public static Gdk.Pixbuf StreamToPixbuf(Stream imageStream)
        {
            return ImageToPixbuf(new Bitmap(imageStream));
        }

        public static Gdk.Pixbuf ImageToPixbuf(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream);
                return pixbuf;
            }
        }

        public static Gdk.PixbufAnimation StreamToPixbufAnim(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                Gdk.PixbufAnimation pixbuf = new Gdk.PixbufAnimation(stream);
                return pixbuf;
            }
        }

        public static Gdk.Pixbuf GetCharacterPortraitGTK(long charID, ImageRequestSize size)
        {
            //TODO: Investigate more
            // Have to use StreamToPixbuf due to an error in GTK
            return StreamToPixbuf(GetCharacterPortrait(charID, size));
        }

        public static Gdk.Pixbuf GetCorporationLogoGTK(long corpID, ImageRequestSize size)
        {
            return StreamToPixbuf(GetCorporationLogo(corpID, size));
        }

        public static Gdk.Pixbuf GetAllianceLogoGTK(long allianceID, ImageRequestSize size)
        {
            return StreamToPixbuf(GetAllianceLogo(allianceID, size));
        }

        public static Gdk.Pixbuf GetItemImageGTK(long typeID, ImageRequestSize size)
        {
            return StreamToPixbuf(GetItemImage(typeID, size));
        }

        public static Gdk.Pixbuf GetItemRenderGTK(long typeID, ImageRequestSize size)
        {
            return StreamToPixbuf(GetItemRender(typeID, size));
        }
		#endregion

        /// <summary>
        /// Returns the character portrait for the given ID, at the requested size
        /// </summary>
        /// <param name="charID">The Character's ID</param>
        /// <param name="size">The Size of the image wanted</param>
        /// <returns>null on error, or the requested Image MemoryStream</returns>
        public static MemoryStream GetCharacterPortrait(long charID, ImageRequestSize size)
        {
            string id = charID > 0 ? charID.ToString() : "1";
            string url = IMAGE_API_URL + "Character/" + id;

            switch (size)
            {
                case ImageRequestSize.Size30x30:
                    url += "_30";
                    break;
                case ImageRequestSize.Size32x32:
                    url += "_32";
                    break;
                case ImageRequestSize.Size64x64:
                    url += "_64";
                    break;
                case ImageRequestSize.Size128x128:
                    url += "_128";
                    break;
                case ImageRequestSize.Size200x200:
                    url += "_200";
                    break;
                case ImageRequestSize.Size256x256:
                    url += "_256";
                    break;
                case ImageRequestSize.Size512x512:
                    url += "_512";
                    break;
                case ImageRequestSize.Size1024x1024:
                    url += "_1024";
                    break;
            }

            url += ".jpg";

            return GetImage(url);
        }

        /// <summary>
        /// Returns the Corporation logo for the given ID, at the requested size
        /// </summary>
        /// <param name="charID">The Corporation ID</param>
        /// <param name="size">The Size of the image wanted</param>
        /// <returns>null on error, or the requested Image MemoryStream</returns>
        public static MemoryStream GetCorporationLogo(long corpID, ImageRequestSize size)
        {
            string url = IMAGE_API_URL + "Corporation/" + corpID.ToString();

            switch (size)
            {
                case ImageRequestSize.Size30x30:
                    url += "_30";
                    break;
                case ImageRequestSize.Size32x32:
                    url += "_32";
                    break;
                case ImageRequestSize.Size64x64:
                    url += "_64";
                    break;
                case ImageRequestSize.Size128x128:
                case ImageRequestSize.Size200x200:
                    url += "_128";
                    break;
                case ImageRequestSize.Size256x256:
                case ImageRequestSize.Size512x512:
                case ImageRequestSize.Size1024x1024:
                    url += "_256";
                    break;
            }

            url += ".png";

            return GetImage(url);
        }

        /// <summary>
        /// Returns the Alliance logo for the given ID, at the requested size
        /// </summary>
        /// <param name="charID">The Alliance ID</param>
        /// <param name="size">The Size of the image wanted</param>
        /// <returns>null on error, or the requested Image MemoryStream</returns>
        public static MemoryStream GetAllianceLogo(long allianceID, ImageRequestSize size)
        {
            string url = IMAGE_API_URL + "Alliance/" + allianceID.ToString();

            switch (size)
            {
                case ImageRequestSize.Size30x30:
                    url += "_30";
                    break;
                case ImageRequestSize.Size32x32:
                    url += "_32";
                    break;
                case ImageRequestSize.Size64x64:
                    url += "_64";
                    break;
                case ImageRequestSize.Size128x128:
                case ImageRequestSize.Size200x200:
                case ImageRequestSize.Size256x256:
                case ImageRequestSize.Size512x512:
                case ImageRequestSize.Size1024x1024:
                    url += "_128";
                    break;
            }

            url += ".png";

            return GetImage(url);
        }

        /// <summary>
        /// Returns the Item Image for the given ID, at the requested size
        /// </summary>
        /// <param name="charID">The Item's type ID</param>
        /// <param name="size">The Size of the image wanted</param>
        /// <returns>null on error, or the requested Image MemoryStream</returns>
        public static MemoryStream GetItemImage(long typeID, ImageRequestSize size)
        {
            string url = IMAGE_API_URL + "InventoryType/" + typeID.ToString();

            switch (size)
            {
                case ImageRequestSize.Size30x30:
                case ImageRequestSize.Size32x32:
                    url += "_32";
                    break;
                case ImageRequestSize.Size64x64:
                case ImageRequestSize.Size128x128:
                case ImageRequestSize.Size200x200:
                case ImageRequestSize.Size256x256:
                case ImageRequestSize.Size512x512:
                case ImageRequestSize.Size1024x1024:
                    url += "_64";
                    break;
            }

            url += ".png";

            return GetImage(url);
        }

        /// <summary>
        /// Gets the item render for a given TypeID
        /// </summary>
        /// <param name="typeID">Type ID of the item</param>
        /// <param name="size">Requested size of the image</param>
        /// <returns>null on error, or the requested Image MemoryStream</returns>
        public static MemoryStream GetItemRender(long typeID, ImageRequestSize size)
        {
            string url = IMAGE_API_URL + "Render/" + typeID.ToString();

            switch (size)
            {
                case ImageRequestSize.Size30x30:
                case ImageRequestSize.Size32x32:
                    url += "_32";
                    break;
                case ImageRequestSize.Size64x64:
                    url += "_64";
                    break;
                case ImageRequestSize.Size128x128:
                    url += "_128";
                    break;
                case ImageRequestSize.Size200x200:
                case ImageRequestSize.Size256x256:
                    url += "_256";
                    break;
                case ImageRequestSize.Size512x512:
                case ImageRequestSize.Size1024x1024:
                    url += "_512";
                    break;
            }

            url += ".png";

            return GetImage(url);
        }

        /// <summary>
        /// Gets an image from the supplied url
        /// </summary>
        /// <param name="url">URL where the image lies</param>
        /// <returns>The image, or null if not found.</returns>
        static public MemoryStream GetImage(string url)
        {
            WebClient webClient = new WebClient();
            WebProxy myProxy = new WebProxy();
            MemoryStream imgStream = null;
            string filePath = IMAGE_CACHE_DIR + url.Substring(IMAGE_API_URL.Length);

            if(File.Exists(filePath))
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                imgStream = new MemoryStream(bytes);
            }
            else
            {
                // TODO: Download Form Interface
                //NeoComm.Forms.DownloadDialog downloadInfoForm = new NeoComm.Forms.DownloadDialog();

                //downloadInfoForm.Show();
                //downloadInfoForm.StatusText = "Downloading from " + url;

                // TODO: Set up the proxy info
                //if (proxyInfo.UseProxy)
                //{
                //    // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                //    myProxy.Address = new Uri("http://" + proxyInfo.ProxyIP + ":" + proxyInfo.ProxyPort);

                //    // Create a NetworkCredential object and associate it with the Proxy property of request object.
                //    if (proxyInfo.ProxyDomain.Length > 0)
                //        myProxy.Credentials = new NetworkCredential(proxyInfo.ProxyUser, proxyInfo.ProxyPass, proxyInfo.ProxyDomain);
                //    else
                //        myProxy.Credentials = new NetworkCredential(proxyInfo.ProxyUser, proxyInfo.ProxyPass);

                //    webClient.Proxy = myProxy;
                //}

                m_IsRetrieving = true;
            
                try
                {
                    // Download image
                    imgStream = new MemoryStream(webClient.DownloadData(url));

                    // Save to image cache
                    string dirName = Path.GetDirectoryName(filePath);

                    if (Directory.Exists(dirName) == false)
                        Directory.CreateDirectory(dirName);

                    SaveStreamToFile(filePath, imgStream);
                }
                catch
                {
                    //MessageBox.Show("Error retrieving icon " + url);
                    Console.WriteLine("Error retrieving icon " + url);
                }

                m_IsRetrieving = false;
                //downloadInfoForm.Close();
            }

            return imgStream;
        }

        /// <summary>
        /// Saves a stream to a file
        /// </summary>
        /// <param name="fileFullPath">File to save stream to</param>
        /// <param name="stream">Stream of data to save</param>
        private static void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream == null || stream.Length == 0)
                return;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}
