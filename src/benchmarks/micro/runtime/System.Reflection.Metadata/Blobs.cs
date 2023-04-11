// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BenchmarkDotNet.Attributes;
using MicroBenchmarks;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using static System.Reflection.Metadata.BlobBuilder;

namespace System.Reflection.Metadata
{
    [BenchmarkCategory(Categories.Runtime, Categories.ReflectionMetadata)]
    public class Blobs
    {
        private const int BlobSize = 1000;
        private const int BlobAdds = 100;
        private static byte[] s_data1 = new byte[BlobSize];

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < BlobSize; i++)
            {
                s_data1[i] = (byte)(i & 0xFF);
            }
        }

        private static BlobBuilder GetBlobBuilder(int extra = 0)
        {
            var blob = new BlobBuilder(BlobSize + sizeof(byte) + sizeof(int) + sizeof(long) + extra);
            blob.WriteByte(10);
            blob.WriteInt32(11);
            blob.WriteInt64(12);
            blob.WriteBytes(s_data1);

            return blob;
        }

        [Benchmark]
        public void Write_NoReuse()
        {
            MetadataBuilder builder = new(BlobSize);
            for (int i = 0; i < BlobAdds; i++)
            {
                BlobBuilder blob = GetBlobBuilder(extra: sizeof(int));
                blob.WriteInt32(i); // Cause a diff
                builder.GetOrAddBlob(blob);
            }
        }

        [Benchmark]
        public void Write_Reuse()
        {
            MetadataBuilder builder = new();
            for (int i = 0; i < BlobAdds; i++)
            {
                builder.GetOrAddBlob(GetBlobBuilder());
            }
        }
    }
}
