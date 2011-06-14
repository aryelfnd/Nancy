﻿namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::DotLiquid;
    using global::DotLiquid.FileSystems;
 
    public class DotLiquidViewEngine : IViewEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        public DotLiquidViewEngine()
            : this(new LiquidNancyFileSystem(string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        public DotLiquidViewEngine(IFileSystem fileSystem)
        {
            if (fileSystem != null)
            {
                Template.FileSystem = fileSystem;
            }
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { yield return "liquid"; }
        }

        public void Initialize(IEnumerable<ViewLocationResult> matchingViews)
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return stream =>
            {
                var hashedModel = 
                    Hash.FromAnonymousObject(new { model = new DynamicDrop(model) });

                var parsed = renderContext.ViewCache.GetOrAdd(
                    viewLocationResult,
                    x => Template.Parse(viewLocationResult.Contents.Invoke().ReadToEnd()));

                var rendered = parsed.Render(hashedModel);

                var writer = new StreamWriter(stream);

                writer.Write(rendered);
                writer.Flush();
            };
        }
    }
}
