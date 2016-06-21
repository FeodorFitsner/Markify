﻿using System;
using System.IO;

using Markify.Core.Rendering;

using static Markify.Models.Document;

namespace Markify.Services.IO
{
    public sealed class FileWriter : IPageWriter
    {
        #region Write Methods

        public void Write(string text, Page page, Uri root)
        {
            var pageFolder = new Uri(root, page.Folder);
            var fullPath = Path.Combine(pageFolder.AbsolutePath, page.Name);
            File.WriteAllText(fullPath, text);
        }

        #endregion
    }
}