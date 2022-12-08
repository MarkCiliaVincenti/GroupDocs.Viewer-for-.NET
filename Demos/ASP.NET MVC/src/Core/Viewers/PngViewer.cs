﻿using System.IO;
using System.Threading.Tasks;
using GroupDocs.Viewer.AspNetMvc.Core.Configuration;
using GroupDocs.Viewer.AspNetMvc.Core.Entities;
using GroupDocs.Viewer.AspNetMvc.Core.FileTypeResolution;
using GroupDocs.Viewer.AspNetMvc.Core.Licensing;
using GroupDocs.Viewer.AspNetMvc.Core.Viewers.Extensions;
using GroupDocs.Viewer.Options;
using Page = GroupDocs.Viewer.AspNetMvc.Core.Entities.Page;

namespace GroupDocs.Viewer.AspNetMvc.Core.Viewers
{
    internal class PngViewer : BaseViewer
    {
        private readonly ViewerConfig _config;

        public PngViewer(ViewerConfig config,
            IViewerLicenser licenser, 
            IFileStorage fileStorage, 
            IFileTypeResolver fileTypeResolver, 
            IPageFormatter pageFormatter)
            : base(config, licenser, fileStorage, fileTypeResolver, pageFormatter)
        {
            _config = config;
        }

        public override string PageExtension => PngPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new PngPage(pageNumber, data);

        public override Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName) => 
            throw new System.NotImplementedException(
                $"{nameof(PngViewer)} does not support retrieving external HTML resources.");

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var pageStream = new MemoryStream();
            var viewOptions = CreateViewOptions(pageStream);

            viewer.View(viewOptions, pageNumber);

            var bytes = pageStream.ToArray();
            var page = CreatePage(pageNumber, bytes);

            return page;
        }

        protected override ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromJpgViewOptions(_config.JpgViewOptions);

        private PngViewOptions CreateViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new PngViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.PngViewOptions);

            return viewOptions;
        }
    }
}