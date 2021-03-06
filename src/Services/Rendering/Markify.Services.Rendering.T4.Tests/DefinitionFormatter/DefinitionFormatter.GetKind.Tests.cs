﻿using System;
using Markify.Domain.Compiler;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetKind_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetKind(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Class, null, "class")]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Interface, null, "interface")]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Struct, null, "struct")]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Delegate, null, "delegate")]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Enum, null, "enum")]
        public void GetKind_ShouldReturnCorrectKeyword(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(expected);
        }
    }
}