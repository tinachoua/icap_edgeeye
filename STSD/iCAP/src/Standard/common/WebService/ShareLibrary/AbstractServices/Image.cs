using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.Interface;
using ShareLibrary.AdminDB;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace ShareLibrary.AbstractServices
{
    public abstract class Image : IImages
    {
        public virtual bool AllowedFileExtensions(List<IFormFile> files)
        {
            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                string ext = filename.Substring(filename.LastIndexOf('.')).ToLower();
                if (!AllowedFileExtensions.Contains(ext))
                {
                    return false;
                }
            };
            return true;
        }

        public virtual bool AllowedFileExtensions(IFormFile file)
        {
            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };
            var filename = ContentDispositionHeaderValue
                            .Parse(file.ContentDisposition)
                            .FileName
                            .Trim('"');
            string ext = filename.Substring(filename.LastIndexOf('.')).ToLower();
            if (!AllowedFileExtensions.Contains(ext))
            {
                return false;
            }
            return true;
        }
        public virtual string GetImgBase64(string Query , string path)
        {
            icapContext ic = new icapContext();
            string photoURL = null;
            byte[] imageBytes;

            try
            {
                switch (path)
                {
                    case "devices":
                        Device dev = ic.Device.Where(d => d.Name == Query).SingleOrDefault();
                        photoURL = dev.PhotoUrl;
                        break;
                    case "branches":
                        Branch bra = ic.Branch.Where(d => d.Id == Int32.Parse(Query)).SingleOrDefault();
                        photoURL = bra.PhotoUrl;
                        break;
                }

                imageBytes = System.IO.File.ReadAllBytes("/var/images/" + photoURL);
                string ext = photoURL.Substring(photoURL.LastIndexOf('.')+1);

                return "data:image/" + ext + ";base64," + Convert.ToBase64String(imageBytes);
                
            }
            catch(Exception)
            {
                imageBytes = System.IO.File.ReadAllBytes("/var/images/" + path + "/00.png");
                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);                
            }            
        }

        //public virtual string GetImgBase64(int devId)
        //{
        //    icapContext ic = new icapContext();
        //    Device dev = ic.Device.Where(d => d.Id == devId).SingleOrDefault();
        //    string photoURL = dev.PhotoUrl;
        //    byte[] imageBytes;

        //    try
        //    {
        //        if (photoURL == null)
        //        {
        //            imageBytes = System.IO.File.ReadAllBytes("/var/images/devices/00.png");
        //            return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        //        }
        //        else
        //        {
        //            try
        //            {
        //                imageBytes = System.IO.File.ReadAllBytes("/var/images/" + photoURL);
        //            }
        //            catch (Exception)
        //            {
        //                imageBytes = System.IO.File.ReadAllBytes("/var/images/devices/00.png");
        //                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        //            }
        //        }

        //        string ext = photoURL.Substring(photoURL.LastIndexOf('.'));

        //        switch (ext)
        //        {
        //            case ".jpeg":
        //                return "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
        //            case ".jpg":
        //                return "data:image/jpg;base64," + Convert.ToBase64String(imageBytes);
        //            case ".bmp":
        //                return "data:image/bmp;base64," + Convert.ToBase64String(imageBytes);
        //            case ".png":
        //                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        //            case ".gif":
        //                return "data:image/gif;base64," + Convert.ToBase64String(imageBytes);
        //            default:
        //                imageBytes = System.IO.File.ReadAllBytes("/var/images/devices/00.png");
        //                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public virtual bool UploadImg(List<IFormFile> files, bool overwrite, string Query, string path)
        {
            icapContext ic = new icapContext();
            string photoURL = null;

            try
            {
                switch (path)
                {
                    case "devices":
                        Device dev = ic.Device.Where(d => d.Name == Query).SingleOrDefault();
                        photoURL = dev.PhotoUrl;
                        foreach (var file in files)
                        {
                            var filename = ContentDispositionHeaderValue
                                             .Parse(file.ContentDisposition)
                                             .FileName
                                             .Trim('"');

                            if (photoURL != null)
                            {
                                string dbfileName = photoURL.Substring((photoURL.LastIndexOf('/')) + 1);
                                if (dbfileName == filename && !overwrite)
                                    return false;//file exists and not overwrite
                            }

                            string filePath = "/var/images/devices" + $@"/{filename}";
                            using (FileStream fs = System.IO.File.Create(filePath))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            dev.PhotoUrl = "devices/" + filename;
                            ic.Device.Update(dev);
                            ic.SaveChanges();
                        }
                        break;

                    case "branches":
                        Branch bra = ic.Branch.Where(d => d.Id ==Int32.Parse(Query)).SingleOrDefault();
                        photoURL = bra.PhotoUrl;
                        foreach (var file in files)
                        {
                            var filename = ContentDispositionHeaderValue
                                             .Parse(file.ContentDisposition)
                                             .FileName
                                             .Trim('"');

                            if (photoURL != null)
                            {
                                string dbfileName = photoURL.Substring((photoURL.LastIndexOf('/')) + 1);
                                if (dbfileName == filename && !overwrite)
                                    return false;//file exists and not overwrite
                            }

                            string filePath = "/var/images/branches" + $@"/{filename}";
                            using (FileStream fs = System.IO.File.Create(filePath))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            bra.PhotoUrl = "branches/" + filename;
                            ic.Branch.Update(bra);
                            ic.SaveChanges();

                        }
                        break;
                }
                return true;
            }
            catch(Exception)
            {
                throw;
            }           
        }

        //public virtual bool UploadImg(List<IFormFile> files, bool overwrite, int devId)
        //{
        //    icapContext ic = new icapContext();
        //    Device dev = ic.Device.Where(d => d.Id == devId).SingleOrDefault();
        //    //if (dev == null)
        //    //    return "Device Not Found";
        //    string photoURL = dev.PhotoUrl;

        //    foreach (var file in files)
        //    {
        //        var filename = ContentDispositionHeaderValue
        //                         .Parse(file.ContentDisposition)
        //                         .FileName
        //                         .Trim('"');

        //        if (photoURL != null)
        //        {
        //            string dbfileName = photoURL.Substring((photoURL.LastIndexOf('/')) + 1);
        //            if (dbfileName == filename && !overwrite)
        //                return false;//file exists and not overwrite
        //        }
        //        try
        //        {
        //            string filePath = "/var/images/devices" + $@"/{filename}";
        //            using (FileStream fs = System.IO.File.Create(filePath))
        //            {
        //                file.CopyTo(fs);
        //                fs.Flush();
        //            }

        //            dev.PhotoUrl = "devices/" + filename;
        //            ic.Device.Update(dev);
        //            ic.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //    return true;
        //}
    }
}
