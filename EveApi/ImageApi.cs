using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;

namespace EveApi
{
    public class ImageApi
    {
        static string m_ImageApiUrl = "http://image.eveonline.com/";
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

        /// <summary>
        /// Returns the character portrait for the given ID, at the requested size
        /// </summary>
        /// <param name="charID">The Character's ID</param>
        /// <param name="size">The Size of the image wanted</param>
        /// <returns>null on error, or the requested Image</returns>
        public static Bitmap GetCharacterPortrait(int charID, ImageRequestSize size)
        {
            string url = m_ImageApiUrl + "/Character/" + charID.ToString();

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
        /// <returns>null on error, or the requested Image</returns>
        public static Bitmap GetCorporationLogo(int corpID, ImageRequestSize size)
        {
            string url = m_ImageApiUrl + "/Corporation/" + corpID.ToString();

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
        /// <returns>null on error, or the requested Image</returns>
        public static Bitmap GetAllianceLogo(int allianceID, ImageRequestSize size)
        {
            string url = m_ImageApiUrl + "/Alliance/" + allianceID.ToString();

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
        /// <returns>null on error, or the requested Image</returns>
        public static Bitmap GetItemImage(int typeID, ImageRequestSize size)
        {
            string url = m_ImageApiUrl + "/InventoryType/" + typeID.ToString();

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
        /// <returns>null on error, or the requested Image</returns>
        public static Bitmap GetItemRender(int typeID, ImageRequestSize size)
        {
            string url = m_ImageApiUrl + "/Render/" + typeID.ToString();

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
        static public Bitmap GetImage(string url)
        {
            WebClient webClient = new WebClient();
            WebProxy myProxy = new WebProxy();
            Bitmap newImage = null;
            
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

            try
            {
                MemoryStream imgStream = new MemoryStream(webClient.DownloadData(url));
                newImage = new System.Drawing.Bitmap(imgStream);


            }
            catch
            {
                //MessageBox.Show("Error retrieving icon " + url);
                Console.WriteLine("Error retrieving icon " + url);
            }

            //downloadInfoForm.Close();
            return newImage;
        }
    }
}
