﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exilence.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Exilence.Helper
{
    public static class CompressionHelper
    {
        public static string Compress<T>(T input)
        {
            var jsonString = JsonConvert.SerializeObject(input, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var buffer = Encoding.UTF8.GetBytes(jsonString);

            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    gZipStream.Write(buffer, 0, buffer.Length);

                var outputBytes = outputStream.ToArray();

                var base64String = Convert.ToBase64String(outputBytes);

                return base64String;
            }
        }

        public static T Decompress<T>(string input)
        {
            string jsonString = null;


            byte[] inputBytes = Convert.FromBase64String(input);

            using (var inputStream = new MemoryStream(inputBytes))
            using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gZipStream))
            {
                jsonString = streamReader.ReadToEnd();

            }

            return JsonConvert.DeserializeObject<T>(jsonString);

        }
    }
}
