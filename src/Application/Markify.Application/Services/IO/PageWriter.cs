﻿using System;
using System.IO;
using Markify.Domain.Document;
using Markify.Domain.Rendering;

namespace Markify.Application.Services.IO
{
    public sealed class PageWriter : IPageWriter
    {
        #region Write Methods

        public void Write(string text, Page page, Uri root)
        {
            var pageFolder = new Uri(root, page.Folder);
            if (!Directory.Exists(pageFolder.AbsolutePath))
                Directory.CreateDirectory(pageFolder.AbsolutePath);

            var fullPath = Path.Combine(pageFolder.AbsolutePath, page.Name);
            File.WriteAllText(fullPath, text);
        }

        #endregion
    }
}