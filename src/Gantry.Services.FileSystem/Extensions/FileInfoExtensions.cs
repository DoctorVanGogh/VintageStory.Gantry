﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Gantry.Services.FileSystem.Extensions
{
    /// <summary>
    ///     Until .NET6 is available for the game, these async helper methods will have to suffice.
    ///
    ///     .NET 6.0 will introduce File.WriteAllTextAsync, and similar IO static async methods.
    /// </summary>
    public static class FileInfoExtensions
    {
        private const int DefaultBufferSize = 4096;

        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        /// <summary>
        ///     Asynchronously opens the file, writes all given bytes to the file, overwriting all contents, then closes the file.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <param name="bytes">The byte array to write to the file.</param>
        public static Task WriteAllBytesAsync(this FileInfo fileInfo, byte[] bytes)
        {
            using var sourceStream = new FileStream(fileInfo.FullName, FileMode.Append, FileAccess.Write, FileShare.None, DefaultBufferSize, true);
            return sourceStream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///     Asynchronously opens the file, writes all given text to the file, overwriting all contents, then closes the file.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <param name="text">The string to write to the file.</param>
        public static Task WriteAllTextAsync(this FileInfo fileInfo, string text)
        {
            var encodedText = Encoding.Unicode.GetBytes(text);
            return fileInfo.WriteAllBytesAsync(encodedText);
        }

        /// <summary>
        ///     Read all lines from a file, as an asynchronous operation.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <returns>A enumerable collection of strings, containing the contents of each line of text within the file.</returns>
        public static async Task<IEnumerable<string>> ReadAllLinesAsync(this FileInfo fileInfo)
        {
            var lines = new List<string>();
            using var sourceStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions);
            using var reader = new StreamReader(sourceStream, Encoding.Unicode);
            while (await reader.ReadLineAsync() is { } line) lines.Add(line);
            return lines;
        }

        /// <summary>
        ///     Read all bytes from a file, as an asynchronous operation.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <returns>A byte array, containing the contents of the file.</returns>
        public static async Task<byte[]> ReadAllBytesAsync(this FileInfo fileInfo)
        {
            using var sourceStream = File.Open(fileInfo.FullName, FileMode.Open);
            var result = new byte[sourceStream.Length];
            var _ = await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);
            return result;
        }

        /// <summary>
        ///     Read all text from a file, as an asynchronous operation.    
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <returns>A string, containing the contents of the file.</returns>
        public static Task<string> ReadAllTextAsync(this FileInfo fileInfo)
        {
            using var sourceStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions);
            using var reader = new StreamReader(sourceStream);
            return reader.ReadToEndAsync();
        }

        /// <summary>
        ///     Read all text from a file, as an asynchronous operation.    
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> wrapper for the file to write to.</param>
        /// <returns>A string, containing the contents of the file.</returns>
        public static string ReadAllText(this FileInfo fileInfo)
        {
            return File.ReadAllText(fileInfo.FullName);
        }

        /// <summary>
        ///     Deserialises the specified file as a collection of a strongly-typed object.
        ///     The consuming type must have a paramaterless constructor.
        /// </summary>
        /// <typeparam name="TModel">The type of object to deserialise into.</typeparam>
        /// <returns>An instance of type <see cref="IEnumerable{TModel}"/>, populated with data from this file.</returns>
        public static IEnumerable<TModel> ParseAsMany<TModel>(this FileInfo fileInfo) where TModel : class, new()
        {
            try
            {
                return JsonConvert.DeserializeObject<IEnumerable<TModel>>(File.ReadAllText(fileInfo.FullName));
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
