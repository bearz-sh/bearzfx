// Copyright © 2017 - 2021 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/xml/XmlCData.cs

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cocoa.Xml;

/// <summary>
///   Xml CData auto conversion.
/// </summary>
/// <remarks>
///   Based on http://stackoverflow.com/a/19832309/18475.
/// </remarks>
public class XmlCData : IXmlSerializable
{
    private string value;

    public XmlCData()
        : this(string.Empty)
    {
    }

    public XmlCData(string value)
    {
        this.value = value;
    }

    public static implicit operator XmlCData(string value)
    {
        return new XmlCData(value);
    }

    public static implicit operator string(XmlCData cdata)
    {
        return cdata.ToSafeString();
    }

    public override string ToString()
    {
        return this.value.ToSafeString();
    }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        this.value = reader.ReadElementString();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteCData(this.value);
    }
}