using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ShareLibrary.Interface
{   
    public interface IImages
    {
        bool AllowedFileExtensions(List<IFormFile> files);
        bool AllowedFileExtensions(IFormFile file);
        string GetImgBase64(string devName , string path);
        //bool UploadImg(List<IFormFile> files, bool overwrite, int devId);
        bool UploadImg(List<IFormFile> files, bool overwrite, string Query, string path);
    }
}
