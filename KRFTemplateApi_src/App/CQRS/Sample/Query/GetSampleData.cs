﻿namespace KRFTemplateApi.App.CQRS.Sample.Query
{
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    
    using Newtonsoft.Json;
    
    using KRFCommon.CQRS.Query;
    using KRFCommon.CQRS.Common;
    using KRFCommon.Context;

    using KRFTemplateApi.Domain.CQRS.Sample.Query;
    using KRFTemplateApi.App.DatabaseQueries;
    using System.Collections.Generic;

    public class GetSampleData : IQuery<SampleInput, SampleOutput>
    {
        private IUserContext _userContext;

        private ISampleDatabaseQuery _sampleDB;
        public GetSampleData( IUserContext userContext, Lazy<ISampleDatabaseQuery> sampleDB )
        {
            this._userContext = userContext;
            this._sampleDB = sampleDB.Value;
        }

        private async Task<SampleOutputItem> GetNextValue(Random rng, int index)
        {
            var temp = rng.Next(-60, 60);
            var dbResult = await this._sampleDB.GetSampleFromTemperatureAsync(temp);
            return new SampleOutputItem
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temp,
                Summary = dbResult != null ? string.Format("{0} -> {1}", dbResult.Code, dbResult.Description) : "Temperature summary not found",
                UserData = this._userContext != null && this._userContext.Claim != Claims.NotLogged ? JsonConvert.SerializeObject(this._userContext) : "No User"
            };
        }

        private async Task<IEnumerable<SampleOutputItem>> MakeDataResult()
        {
            var rng = new Random();
            var output = new List<SampleOutputItem>();

            for(int i = 1; i<= 5; i++)
            {
                var r = await this.GetNextValue(rng, i);
                output.Add(r);
            }

            return output;
        }

        public async Task<IResponseOut<SampleOutput>> QueryAsync(SampleInput request)
        {
            var result = await this.MakeDataResult();
            
            if(result == null || !result.Any())
            {
                return ResponseOut<SampleOutput>.GenerateFault(new ErrorOut(System.Net.HttpStatusCode.BadRequest, "Error Ocurred, no results", ResponseErrorType.Unknown));
            }

            return ResponseOut<SampleOutput>.GenerateResult(new SampleOutput { 
                Output = result
            });
        }
    }
}