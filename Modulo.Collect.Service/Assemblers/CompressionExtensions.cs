/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Modulo.Collect.Service.Assemblers
{
    public static class CompressionExtensions
    {
        /// <summary>
        /// Returns the byte array of a compressed string
        /// </summary>
        public static byte[] ToCompressedByteArray(this string source)
        {
            // convert the source string into a memory stream
            using (
                MemoryStream inMemStream = new MemoryStream(Encoding.ASCII.GetBytes(source)),
                             outMemStream = new MemoryStream())
            {
                // create a compression stream with the output stream
                using (var zipStream = new DeflateStream(outMemStream, CompressionMode.Compress, true))
                    // copy the source string into the compression stream
                    inMemStream.WriteTo(zipStream);

                // return the compressed bytes in the output stream
                return outMemStream.ToArray();
            }
        }
        /// <summary>
        /// Returns the base64 encoded string for the compressed byte array of the source string
        /// </summary>
        public static string ToCompressedBase64String(this string source)
        {
            return Convert.ToBase64String(source.ToCompressedByteArray());
        }

        /// <summary>
        /// Returns the original string for a compressed base64 encoded string
        /// </summary>
        public static string ToUncompressedString(this string source)
        {
            // get the byte array representation for the compressed string
            var compressedBytes = Convert.FromBase64String(source);

            // load the byte array into a memory stream
            using (var inMemStream = new MemoryStream(compressedBytes))
                // and decompress the memory stream into the original string
            using (var decompressionStream = new DeflateStream(inMemStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressionStream))
                return streamReader.ReadToEnd();
        }
    }
}
