/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Modulo.Collect.OVAL.Definitions.helpers
{
	public static class XmlSignatureHelper
	{
		public static bool VerifySignature(string xml, out X509Certificate2 certificate)
		{
			using (StringReader sr = new StringReader(xml))
			{
				using (XmlReader reader = XmlReader.Create(sr, new XmlReaderSettings()
				{
					CloseInput = true,
					ValidationType = ValidationType.None,
				}))
				{
					return VerifySignature(reader, out certificate);
				}
			}
		}

		public static bool VerifySignature(XmlReader reader, out X509Certificate2 certificate)
		{
			XmlDocument document = new XmlDocument();
			document.PreserveWhitespace = false;
			document.Load(reader);

			SignedXml signedXml = new SignedXml(document);
			XmlNodeList nodeList = document.GetElementsByTagName("Signature");

			certificate = null;

			if (nodeList.Count == 1)
			{
				signedXml.LoadXml((XmlElement)nodeList[0]);
				certificate = signedXml.KeyInfo.OfType<KeyInfoX509Data>().SelectMany(c => c.Certificates.OfType<X509Certificate2>()).FirstOrDefault();
				return signedXml.CheckSignature();
			}

			return false;
		}

		public static bool VerifySignature(string xml, IEnumerable<X509Certificate2> trustedCertificates)
		{
			using (StringReader sr = new StringReader(xml))
			{
				using (XmlReader reader = XmlReader.Create(sr))
				{
					return VerifySignature(reader, trustedCertificates);
				}
			}
		}

		public static bool VerifySignature(XmlReader reader, IEnumerable<X509Certificate2> trustedCertificates)
		{
			X509Certificate2 certificate;
			if (VerifySignature(reader, out certificate))
			{
				return X509Helper.IsCertificateTrusted(certificate, trustedCertificates);
			}

			return false;
		}

		public static bool VerifySignature(Stream xmlStream, Stream signatureStream, IEnumerable<X509Certificate2> trustedCertificates)
		{
			X509Certificate2 certificate;
			if (VerifySignature(xmlStream, signatureStream, out certificate))
			{
				return X509Helper.IsCertificateTrusted(certificate, trustedCertificates);
			}

			return false;
		}

		public static XmlElement Sign(string xml, X509Certificate2 certificate)
		{
			using (StringReader sr = new StringReader(xml))
			{
				using (XmlReader reader = XmlReader.Create(sr))
				{
					return Sign(reader, certificate);
				}
			}
		}

		public static XmlElement Sign(XmlReader xmlReader, X509Certificate2 certificate)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = false;
			xmlDocument.Load(xmlReader);
			XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Signature");
			foreach (var node in nodeList.OfType<XmlNode>().ToList())
			{
				if (node.ParentNode != null)
				{
					node.ParentNode.RemoveChild(node);
				}
			}

			SignedXml signedXml = new SignedXml(xmlDocument);
			signedXml.SigningKey = certificate.PrivateKey;

			KeyInfo keyInfo = new KeyInfo();
			keyInfo.AddClause(new KeyInfoX509Data(certificate));
			signedXml.KeyInfo = keyInfo;

			Reference reference = new Reference(string.Empty);
			XmlDsigEnvelopedSignatureTransform transform = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(transform);

			signedXml.AddReference(reference);
			signedXml.ComputeSignature();

			return signedXml.GetXml();
		}

		public static XmlElement Sign(Stream xmlStream, X509Certificate2 certificate)
		{
			SignedXml signedXml = new SignedXml();
			signedXml.SigningKey = certificate.PrivateKey;

			KeyInfo keyInfo = new KeyInfo();
			keyInfo.AddClause(new KeyInfoX509Data(certificate));
			signedXml.KeyInfo = keyInfo;

			Reference reference = new Reference(xmlStream);
			signedXml.AddReference(reference);

			signedXml.ComputeSignature();

			return signedXml.GetXml();
		}

		public static bool VerifySignature(Stream xmlStream, Stream signatureStream, out X509Certificate2 certificate)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(signatureStream);

			SignedXml signedXml = new SignedXml();
			signedXml.LoadXml(xmlDocument.DocumentElement);
			certificate = signedXml.KeyInfo.OfType<KeyInfoX509Data>().SelectMany(c => c.Certificates.OfType<X509Certificate2>()).FirstOrDefault();

			foreach (Reference reference in signedXml.SignedInfo.References.ToArray())
			{
				Reference newReference = new Reference(xmlStream);
				newReference.LoadXml(reference.GetXml());
				signedXml.AddReference(newReference);
				signedXml.SignedInfo.References.Remove(reference);
			}

			return signedXml.CheckSignature();
		}
	}

	public static class X509Helper
	{
		public static bool IsCertificateTrusted(X509Certificate2 certificate, IEnumerable<X509Certificate2> trustedCertificates)
		{
			if (certificate == null) { throw new ArgumentNullException("certificate"); }
			if (trustedCertificates == null) { throw new ArgumentNullException("trustedCertificates"); }

			X509Chain chain = new X509Chain();
			X509Certificate2[] trusted = trustedCertificates.ToArray();
			chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
			chain.ChainPolicy.ExtraStore.AddRange(trusted);
			chain.Build(certificate);
			return chain.ChainElements.OfType<X509ChainElement>().Any(c => trusted.Select(t => t.Thumbprint).Contains(c.Certificate.Thumbprint));
		}
	}
}
