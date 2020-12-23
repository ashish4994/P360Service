using CreditOne.P360FormSubmissionService.Models;
using CreditOne.P360FormSubmissionService.Services;
using CreditOne.P360FormSubmissionService.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Collections.Generic;
using CreditOne.P360FormSubmissionService.Filters;
using System;
using CreditOne.P360FormSubmissionService.DomainServices.Contracts;
using CreditOne.P360FormSubmissionService.DomainServices;
using CreditOne.P360FormSubmissionService.Infra.Strategies.FileFactories;
using CreditOne.P360FormSubmissionService.Infra.Factories;
using CreditOne.P360FormSubmissionService.Infra.Strategies.Contracts;
using CreditOne.P360FormSubmissionService.Infra.Strategies.ImageExtensionExtractStrategies;

namespace CreditOne.P360FormSubmissionService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IHostingEnvironment env = serviceProvider.GetService<IHostingEnvironment>();

            if (env.IsDevelopment())
            {
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            }
            else
            {
                services.AddMvc(options =>
                {
                    options.Filters.Add(new WorkpacketExceptionFilter());
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            }

            services.Configure<P360LoginData>(Configuration.GetSection("P360LoginData"));
            services.Configure<List<FormToQueue>>(Configuration.GetSection("FormToQueueMapping"));
            services.Configure<P360ServiceData>(Configuration.GetSection("P360ServiceData"));
            services.Configure<DummyFileInformation>(Configuration.GetSection("DummyFileInformation"));
            

            services.AddSingleton(typeof(IP360AuthenticationService), typeof(P360AuthenticationService));
            services.AddSingleton(typeof(IWorkpacketCreationService), typeof(P360WorkpacketCreationService));
            services.AddSingleton(typeof(IWorkpacketUpdateService), typeof(P360WorkpacketUpdateService));
            services.AddSingleton(typeof(IWorkpacketSearchService), typeof(P360WorkpacketSearchService));
            services.AddSingleton(typeof(IWorkpacketManageService), typeof(P360WorkpacketManageService));
            services.AddSingleton(typeof(IWorkpacketUpdateDomainService), typeof(WorkpacketUpdateDomainService));
            services.AddSingleton(typeof(IFormDataDomainService), typeof(FormDataDomainService));
            services.AddSingleton(typeof(IP360SearchDomainService), typeof(P360SearchDomainService));
            services.AddSingleton(typeof(IWorkpacketDeletionService), typeof(P360WorkpacketDeletionService));
            services.AddSingleton(typeof(IFileToWorkpacketUpdateStrategyFactory), typeof(FileNameFileToWorkpacketUpdateStrategyFactory));
            services.AddSingleton(typeof(IFileExtensionExtractStrategy), typeof(ByteExtensionExtractStrategy));
            services.AddSingleton(typeof(IArchiveDomainService), typeof(ArchiveDomainService));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
