﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sigscan.Structs;
using Xunit;

namespace Reloaded.Memory.SigScan.Tests
{
    public class ScannerTests
    {
        byte[] _data =
            {
                // 256 bytes, each row is 16 bytes.
                0xd3, 0xb2, 0x7a, 0x18, 0x57, 0x7b, 0x8a, 0xb8, 0x1e, 0x04, 0x25, 0x12, 0x2b, 0x86, 0xe5, 0xe3,
                0x21, 0xaf, 0xa3, 0xfd, 0x1e, 0x71, 0xd1, 0xd6, 0x69, 0x9f, 0x23, 0x40, 0x8d, 0xb4, 0xe1, 0x3e,
                0xa7, 0x6c, 0xfe, 0xb1, 0xba, 0x7e, 0xe1, 0xde, 0xef, 0xbd, 0xbd, 0x0c, 0xd4, 0xef, 0x5c, 0x60,
                0x6b, 0xd6, 0xab, 0x98, 0xf7, 0x3e, 0x79, 0x9d, 0xfb, 0xfc, 0x0a, 0x77, 0x20, 0x08, 0xf1, 0x36,
                0xb6, 0x9b, 0x21, 0x8c, 0x79, 0x2e, 0x71, 0x6c, 0xf2, 0xb7, 0x61, 0x19, 0x19, 0xcf, 0x52, 0x95,
                0x99, 0xd2, 0x63, 0xb1, 0xd3, 0xea, 0xac, 0x47, 0xe0, 0xae, 0x45, 0x9f, 0xa7, 0x1e, 0xb1, 0xec,
                0x32, 0x10, 0x1d, 0x5e, 0x66, 0xb4, 0xc1, 0x42, 0xd1, 0xc4, 0xfe, 0x6c, 0x55, 0x8f, 0x9c, 0x3b,
                0xcc, 0x3e, 0x31, 0xf2, 0x15, 0x04, 0xfd, 0xea, 0xae, 0x1f, 0x14, 0xf3, 0x4f, 0x3b, 0xb8, 0xdb,
                0xf4, 0xee, 0xbf, 0x99, 0x93, 0xcb, 0x5c, 0x6d, 0xa4, 0xe3, 0x70, 0x09, 0x45, 0xcf, 0x64, 0x98,
                0x19, 0x0e, 0xd2, 0xe0, 0x85, 0x1e, 0xc5, 0x72, 0x54, 0xae, 0xd9, 0xbf, 0x56, 0xdd, 0xdd, 0x6b,
                0x24, 0xa4, 0x2e, 0x70, 0x01, 0x8f, 0x8b, 0x5a, 0x7f, 0xcc, 0x41, 0xc3, 0x9c, 0x60, 0xc2, 0x22,
                0x3f, 0x42, 0x7c, 0x1c, 0x7e, 0xe6, 0xa2, 0x43, 0x6f, 0xda, 0x1a, 0x08, 0x01, 0x87, 0x61, 0x70,
                0x0d, 0xdf, 0xd0, 0xd9, 0x0f, 0xbd, 0x25, 0xee, 0x17, 0x6d, 0x0c, 0x35, 0xd8, 0x76, 0x53, 0xdc,
                0x8f, 0x27, 0x55, 0xf8, 0x13, 0x9d, 0x47, 0x6c, 0x11, 0x7e, 0x06, 0xe5, 0xbd, 0xf7, 0x54, 0xec,
                0x75, 0x0c, 0x06, 0x86, 0x14, 0x3e, 0x19, 0x61, 0x8b, 0xfa, 0x17, 0x0d, 0xc0, 0x5e, 0xf6, 0x03,
                0x07, 0xb3, 0x59, 0xb4, 0xa5, 0xa0, 0x0a, 0x54, 0x76, 0x56, 0x51, 0x45, 0x33, 0x9b, 0x7a, 0xbb
            };

        
        [Fact]
        public void InstantiateFromCurrentProcess()
        {
            // Test fails if function throws.
            var thisProcess = Process.GetCurrentProcess();
            var scanner     = new Scanner(thisProcess, thisProcess.MainModule);

            Assert.NotEmpty(scanner.Data);
            Assert.NotNull (scanner.Data);
        }

        [Fact]
        public void FindBasicPattern()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("04 25 12 2B 86 E5 E3");
            var resultSimple = scanner.SimpleFindPattern("04 25 12 2B 86 E5 E3");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(9, resultCompiled.Offset);
        }

        [Fact]
        public void FindPatternWithMask()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("04 25 ?? ?? 86 E5 E3");
            var resultSimple = scanner.SimpleFindPattern("04 25 ?? ?? 86 E5 E3");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(9, resultCompiled.Offset);
        }

        [Fact]
        public void FindPatternWithLongAndMask()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("04 25 12 2B 86 E5 E3 21 AF A3 ?? ?? 71 D1");
            var resultSimple = scanner.SimpleFindPattern("04 25 12 2B 86 E5 E3 21 AF A3 ?? ?? 71 D1");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultSimple.Found);
            Assert.Equal(9, resultSimple.Offset);
        }

        [Fact]
        public void FindPatternStartingWithSkip()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("?? 25 ?? ?? 86 E5 E3 ?? AF A3 ??");
            var resultSimple   = scanner.SimpleFindPattern("?? 25 ?? ?? 86 E5 E3 ?? AF A3 ??");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(9, resultCompiled.Offset);
        }

        [Fact]
        public void FindPatternAtEndOfData()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("7A BB");
            var resultSimple   = scanner.SimpleFindPattern("7A BB");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(254, resultCompiled.Offset);
        }

        [Fact]
        public void FindFirstByte()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("D3 B2 7A");
            var resultSimple = scanner.SimpleFindPattern("D3 B2 7A");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(0, resultCompiled.Offset);
        }

        [Fact]
        public void FindLastByte()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("BB");
            var resultSimple = scanner.SimpleFindPattern("BB");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.True(resultCompiled.Found);
            Assert.Equal(255, resultCompiled.Offset);
        }

        [Fact]
        public void PatternNotFound()
        {
            var scanner = new Scanner(_data);
            var resultCompiled = scanner.CompiledFindPattern("7A BB CC DD EE FF");
            var resultSimple   = scanner.SimpleFindPattern("7A BB CC DD EE FF");

            Assert.Equal(resultCompiled, resultSimple);
            Assert.False(resultCompiled.Found);
            Assert.Equal(-1, resultCompiled.Offset);
        }
    }
}
