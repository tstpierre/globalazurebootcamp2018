using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Neudesic.Elements.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Isagenix.Qualia.SmartOffice.RoomService.Domain;

namespace Isagenix.Qualia.SmartOffice.RoomService.Domain {
    
    /// <summary>
    /// Implementation of <see cref="ISampleRepository"/> that stores content within Azure Blob Storage.
    /// </summary>
    /// <remarks>
    /// NOTE - By default, this code generates CS1998 warnings. If your code does need to be async, remove the 'async' keyword from calls and call Task.FromResult or Task.CompletedTask (in the case of void) to return an appropriate task.
    /// </remarks>
    internal class SampleRepository
        : ISampleRepository {

        private List<Sample> m_cache = new List<Sample>();

        public async Task<Sample> CreateSample(Sample sample) {
            sample.Id = Guid.NewGuid().ToString();
            m_cache.Add(sample);
            return sample;
        }

        public async Task DeleteSampleById(string id) {
            int index = m_cache.FindIndex(x => x.Id == id);
            if (index > 0) {
                m_cache.RemoveAt(index);
            }
            return;
        }

        public async Task<Sample> FindSampleById(string id) {
            Sample sample = m_cache.Where(x => x.Id == id).FirstOrDefault();
            return sample;
        }

        public async Task<Sample> SaveSample(Sample sample) {
            Sample existingSample = m_cache.Where(x => x.Id == sample.Id).FirstOrDefault();

            if(existingSample != null) {
                existingSample.Metric = sample.Metric;
                existingSample.Value = sample.Value;
            } else {
                m_cache.Add(sample);
            }

            return existingSample;
        }

    }
}
