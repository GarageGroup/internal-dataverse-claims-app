﻿using System;
using System.Net.Mime;
using GarageGroup.Infra;
using Xunit;

namespace GarageGroup.Internal.Dataverse.Claims.Service.DbUserApi.Test;

partial class DbUserApiSource
{
    public static TheoryData<FlatArray<Result<HttpSendOut, HttpSendFailure>>, DbUserSetGetOut> GetSetOutTestData
        =>
        new()
        {
            {
                [
                    new HttpSendOut
                    {
                        StatusCode = HttpSuccessCode.OK,
                        Body = new()
                        {
                            Content = new("""
                                <?xml version="1.0" encoding="utf-8"?>
                                <EnumerationResults ServiceEndpoint="https://stesukhorukovdev.blob.core.windows.net/" ContainerName="dataverse-users">
                                    <MaxResults>2</MaxResults>
                                    <Blobs>
                                        <Blob>
                                            <Name>5c259e2e-e186-45aa-83cf-fafe76dc86f2.txt</Name>
                                            <Properties>
                                                <Creation-Time>Wed, 14 Aug 2024 13:54:06 GMT</Creation-Time>
                                                <Last-Modified>Wed, 14 Aug 2024 13:54:06 GMT</Last-Modified>
                                                <Etag>0x8DCBC68909B940B</Etag>
                                                <Content-Length>1</Content-Length>
                                                <Content-Type>text/plain</Content-Type>
                                                <Content-Encoding />
                                                <Content-Language />
                                                <Content-CRC64 />
                                                <Content-MD5>xMpCOKC5I4INzFCab3WEmw==</Content-MD5>
                                                <Cache-Control />
                                                <Content-Disposition />
                                                <BlobType>BlockBlob</BlobType>
                                                <AccessTier>Hot</AccessTier>
                                                <AccessTierInferred>true</AccessTierInferred>
                                                <LeaseStatus>unlocked</LeaseStatus>
                                                <LeaseState>available</LeaseState>
                                                <ServerEncrypted>true</ServerEncrypted>
                                            </Properties>
                                            <Metadata>
                                                <dataverseuserid>52b24a03-ca3d-474d-b1ca-293a587ef49b</dataverseuserid>
                                                <azureuserid>5c259e2e-e186-45aa-83cf-fafe76dc86f2</azureuserid>
                                            </Metadata>
                                            <OrMetadata />
                                        </Blob>
                                        <Blob>
                                            <Name>d4632940-7676-49bb-9b7a-65b6783eddf1.txt</Name>
                                            <Properties>
                                                <Creation-Time>Wed, 14 Aug 2024 13:46:05 GMT</Creation-Time>
                                                <Last-Modified>Wed, 14 Aug 2024 13:46:05 GMT</Last-Modified>
                                                <Etag>0x8DCBC6771BD20B0</Etag>
                                                <Content-Length>1</Content-Length>
                                                <Content-Type>text/plain</Content-Type>
                                                <Content-Encoding />
                                                <Content-Language />
                                                <Content-CRC64 />
                                                <Content-MD5>xMpCOKC5I4INzFCab3WEmw==</Content-MD5>
                                                <Cache-Control />
                                                <Content-Disposition />
                                                <BlobType>BlockBlob</BlobType>
                                                <AccessTier>Hot</AccessTier>
                                                <AccessTierInferred>true</AccessTierInferred>
                                                <LeaseStatus>unlocked</LeaseStatus>
                                                <LeaseState>available</LeaseState>
                                                <ServerEncrypted>true</ServerEncrypted>
                                            </Properties>
                                            <Metadata>
                                                <dataverseuserid>06a33cca-3762-453f-be15-a1b662c13e03</dataverseuserid>
                                                <azureuserid>d4632940-7676-49bb-9b7a-65b6783eddf1</azureuserid>
                                            </Metadata>
                                            <OrMetadata />
                                        </Blob>
                                    </Blobs>
                                    <NextMarker>2!68!MDAwMDA3ITQ0Ny50eHQhMDAwMDI4ITk5OTktMTItMzFUMjM6NTk6NTkuOTk5OTk5OVoh</NextMarker>
                                </EnumerationResults>
                                """),
                            Type = new(MediaTypeNames.Text.Xml)
                        }
                    },
                    new HttpSendOut
                    {
                        StatusCode = HttpSuccessCode.OK,
                        Body = new()
                        {
                            Content = new("""﻿
                                <?xml version="1.0" encoding="utf-8"?>
                                <EnumerationResults ServiceEndpoint="https://stesukhorukovdev.blob.core.windows.net/" ContainerName="dataverse-users">
                                    <Marker>2!68!MDAwMDA3ITQ0Ny50eHQhMDAwMDI4ITk5OTktMTItMzFUMjM6NTk6NTkuOTk5OTk5OVoh</Marker>
                                    <MaxResults>2</MaxResults>
                                    <Blobs>
                                        <Blob>
                                            <Name>012880a5-ad5c-4f41-8ee7-f1e4a97e22f8.txt</Name>
                                            <Properties>
                                                <Creation-Time>Wed, 14 Aug 2024 13:54:06 GMT</Creation-Time>
                                                <Last-Modified>Wed, 14 Aug 2024 13:54:06 GMT</Last-Modified>
                                                <Etag>0x8DCBC68909B940B</Etag>
                                                <Content-Length>1</Content-Length>
                                                <Content-Type>text/plain</Content-Type>
                                                <Content-Encoding />
                                                <Content-Language />
                                                <Content-CRC64 />
                                                <Content-MD5>xMpCOKC5I4INzFCab3WEmw==</Content-MD5>
                                                <Cache-Control />
                                                <Content-Disposition />
                                                <BlobType>BlockBlob</BlobType>
                                                <AccessTier>Hot</AccessTier>
                                                <AccessTierInferred>true</AccessTierInferred>
                                                <LeaseStatus>unlocked</LeaseStatus>
                                                <LeaseState>available</LeaseState>
                                                <ServerEncrypted>true</ServerEncrypted>
                                            </Properties>
                                            <Metadata>
                                                <dataverseuserid>114deacf-5dcd-ed11-b597-6045bd8959f5</dataverseuserid>
                                                <azureuserid>012880a5-ad5c-4f41-8ee7-f1e4a97e22f8</azureuserid>
                                            </Metadata>
                                            <OrMetadata />
                                        </Blob>
                                    </Blobs>
                                    <NextMarker></NextMarker>
                                </EnumerationResults>
                                """),
                            Type = new(MediaTypeNames.Text.Xml)
                        }
                    }
                ],
                new()
                {
                    Users =
                    [
                        new(
                            azureUserId: new("5c259e2e-e186-45aa-83cf-fafe76dc86f2"),
                            dataverseUserId: new("52b24a03-ca3d-474d-b1ca-293a587ef49b")),
                        new(
                            azureUserId: new("d4632940-7676-49bb-9b7a-65b6783eddf1"),
                            dataverseUserId: new("06a33cca-3762-453f-be15-a1b662c13e03")),
                        new(
                            azureUserId: new("012880a5-ad5c-4f41-8ee7-f1e4a97e22f8"),
                            dataverseUserId: new("114deacf-5dcd-ed11-b597-6045bd8959f5"))
                    ]
                }
            },
        };
}