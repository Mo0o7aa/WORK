using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace ntra_missions
{
    /// <summary>
    /// Builds a Google-Earth KMZ from a complaint's sites and sectors.
    ///
    /// Same concept as the "RF SECTOR TO KMZ WITH LINES" Python tool, but the
    /// input is the complaint already loaded from the database instead of an
    /// Excel sheet: every site becomes a point marker and every sector becomes
    /// a line drawn from the site along the sector's azimuth.
    /// </summary>
    public class GenerateKMZ
    {
        // Sector lines have no length in the data, so use a fixed visual length
        // (metres) — same idea as the "Length (m)" column in the Excel tool.
        private const double SectorLineLengthMeters = 500.0;

        // ---- geodesy helpers (ported from the Python tool) ----

        private static void MetersToDeg(double latDeg, double dxM, double dyM, out double dLat, out double dLon)
        {
            double latRad = latDeg * Math.PI / 180.0;
            double mPerDegLat = 111132.92 - 559.82 * Math.Cos(2 * latRad) + 1.175 * Math.Cos(4 * latRad);
            double mPerDegLon = 111412.84 * Math.Cos(latRad) - 93.5 * Math.Cos(3 * latRad);
            dLat = dyM / mPerDegLat;
            dLon = dxM / mPerDegLon;
        }

        private static void DestPoint(double lat, double lon, double azDeg, double distM, out double outLat, out double outLon)
        {
            double theta = azDeg * Math.PI / 180.0;
            double dx = distM * Math.Sin(theta);
            double dy = distM * Math.Cos(theta);
            double dLat, dLon;
            MetersToDeg(lat, dx, dy, out dLat, out dLon);
            outLat = lat + dLat;
            outLon = lon + dLon;
        }

        // ---- KML building ----

        private static string EscapeXml(string s)
        {
            if (s == null)
                return "";

            return s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("'", "&apos;");
        }

        private static string Coord(double value)
        {
            return value.ToString("0.########", CultureInfo.InvariantCulture);
        }

        // Descriptions contain HTML (<br> line breaks), which is not valid XML
        // content — KML requires it wrapped in a CDATA block.
        private static string WrapCdata(string html)
        {
            return "<![CDATA[" + (html ?? "").Replace("]]>", "]]&gt;") + "]]>";
        }

        private static bool TryParseCoordinate(string text, out double value)
        {
            return double.TryParse(
                (text ?? "").Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static string BuildKml(Complaints complaint)
        {
            StringBuilder kml = new StringBuilder();

            string docName = complaint.GetComplaintId();
            if (string.IsNullOrEmpty(docName))
                docName = "Complaint";

            kml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            kml.Append("<kml xmlns=\"http://www.opengis.net/kml/2.2\">\n");
            kml.Append("<Document>\n");
            kml.Append("<name>").Append(EscapeXml(docName)).Append("</name>\n");
            kml.Append("<Style id=\"site-icon\">\n");
            kml.Append("  <IconStyle><scale>1.2</scale><Icon>\n");
            kml.Append("  <href>http://maps.google.com/mapfiles/kml/shapes/placemark_circle.png</href>\n");
            kml.Append("  </Icon></IconStyle><LabelStyle><scale>0.9</scale></LabelStyle></Style>\n");
            kml.Append("<Style id=\"sector-line\">\n");
            kml.Append("  <LineStyle><color>ff000080</color><width>2</width></LineStyle>\n");
            kml.Append("  <PolyStyle><color>4d000080</color></PolyStyle>\n");
            kml.Append("</Style>\n");

            List<BASIC_STRUCTS.SITE_STRUCT> sites = complaint.GetComplaintsSites();

            for (int i = 0; i < sites.Count; i++)
            {
                BASIC_STRUCTS.SITE_STRUCT site = sites[i];

                double lat, lon;
                if (!TryParseCoordinate(site.latitude, out lat) || !TryParseCoordinate(site.longitude, out lon))
                    continue;

                string siteName = string.IsNullOrEmpty(site.site_number) ? ("Site_" + (i + 1)) : site.site_number;

                // point marker + description summarising the site's sectors
                // (raw text here — the whole block goes inside CDATA below)
                StringBuilder description = new StringBuilder();
                if (!string.IsNullOrEmpty(site.region))
                    description.Append("Region: ").Append(site.region).Append("<br>");
                if (!string.IsNullOrEmpty(site.city))
                    description.Append("City: ").Append(site.city).Append("<br>");

                if (site.sectors != null)
                {
                    for (int j = 0; j < site.sectors.Count; j++)
                    {
                        BASIC_STRUCTS.SECTOR_STRUCT sector = site.sectors[j];

                        List<string> parts = new List<string>();
                        if (!string.IsNullOrEmpty(sector.sector_number))
                            parts.Add("Sector: " + sector.sector_number);
                        parts.Add("Azimuth: " + sector.azimuth + "°");
                        if (sector.rtwp != 0)
                            parts.Add("RTWP: " + sector.rtwp + " dBm");
                        if (sector.sector_bands != null && sector.sector_bands.Count > 0)
                            parts.Add("Bands: " + string.Join(", ", sector.sector_bands));

                        description.Append(string.Join(" | ", parts)).Append("<br>");
                    }
                }

                kml.Append("<Placemark>\n");
                kml.Append("<name>").Append(EscapeXml(siteName)).Append("</name>\n");
                kml.Append("<description>").Append(WrapCdata(description.ToString())).Append("</description>\n");
                kml.Append("<styleUrl>#site-icon</styleUrl>\n");
                kml.Append("<ExtendedData>\n");
                kml.Append("<Data name=\"Site\"><value>").Append(EscapeXml(siteName)).Append("</value></Data>\n");
                kml.Append("<Data name=\"Region\"><value>").Append(EscapeXml(site.region)).Append("</value></Data>\n");
                kml.Append("<Data name=\"City\"><value>").Append(EscapeXml(site.city)).Append("</value></Data>\n");
                kml.Append("</ExtendedData>\n");
                kml.Append("<Point><coordinates>")
                   .Append(Coord(lon)).Append(",").Append(Coord(lat)).Append(",0")
                   .Append("</coordinates></Point>\n");
                kml.Append("</Placemark>\n");

                // one line per sector, drawn along its azimuth.
                // The placemark NAME carries sector number + RTWP so that in
                // Google My Maps "Set labels -> name" shows them on the map;
                // ExtendedData is imported by My Maps as data columns too.
                if (site.sectors != null)
                {
                    for (int j = 0; j < site.sectors.Count; j++)
                    {
                        BASIC_STRUCTS.SECTOR_STRUCT sector = site.sectors[j];

                        double endLat, endLon;
                        DestPoint(lat, lon, sector.azimuth, SectorLineLengthMeters, out endLat, out endLon);

                        List<string> nameParts = new List<string>();
                        if (!string.IsNullOrEmpty(sector.sector_number))
                            nameParts.Add("S" + sector.sector_number);
                        if (sector.rtwp != 0)
                            nameParts.Add(sector.rtwp + " dBm");
                        string lineName = nameParts.Count > 0
                            ? string.Join(" | ", nameParts)
                            : siteName + " (" + sector.azimuth + "°)";

                        string bands = (sector.sector_bands != null && sector.sector_bands.Count > 0)
                            ? string.Join(", ", sector.sector_bands)
                            : "";

                        kml.Append("<Placemark>\n");
                        kml.Append("<name>").Append(EscapeXml(lineName)).Append("</name>\n");
                        kml.Append("<styleUrl>#sector-line</styleUrl>\n");
                        kml.Append("<ExtendedData>\n");
                        kml.Append("<Data name=\"Site\"><value>").Append(EscapeXml(siteName)).Append("</value></Data>\n");
                        kml.Append("<Data name=\"Sector\"><value>").Append(EscapeXml(sector.sector_number)).Append("</value></Data>\n");
                        kml.Append("<Data name=\"Azimuth\"><value>").Append(sector.azimuth).Append("</value></Data>\n");
                        kml.Append("<Data name=\"RTWP\"><value>").Append(sector.rtwp).Append("</value></Data>\n");
                        kml.Append("<Data name=\"Bands\"><value>").Append(EscapeXml(bands)).Append("</value></Data>\n");
                        kml.Append("</ExtendedData>\n");
                        kml.Append("<LineString><tessellate>1</tessellate><coordinates>");
                        kml.Append(Coord(lon)).Append(",").Append(Coord(lat)).Append(",0 ");
                        kml.Append(Coord(endLon)).Append(",").Append(Coord(endLat)).Append(",0");
                        kml.Append("</coordinates></LineString>\n");
                        kml.Append("</Placemark>\n");
                    }
                }
            }

            kml.Append("</Document>\n</kml>\n");
            return kml.ToString();
        }

        private static void WriteKmz(string kml, string outPath)
        {
            if (!outPath.ToLowerInvariant().EndsWith(".kmz"))
                outPath += ".kmz";

            if (File.Exists(outPath))
                File.Delete(outPath);

            using (FileStream fs = new FileStream(outPath, FileMode.Create))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                ZipArchiveEntry entry = zip.CreateEntry("doc.kml", CompressionLevel.Optimal);
                using (StreamWriter writer = new StreamWriter(entry.Open(), new UTF8Encoding(false)))
                {
                    writer.Write(kml);
                }
            }
        }

        /// <summary>
        /// Prompts for a location and writes the complaint as a KMZ file ready
        /// to open in Google Earth / upload to Google Maps.
        /// </summary>
        public void ExportComplaint(Complaints complaint)
        {
            List<BASIC_STRUCTS.SITE_STRUCT> sites = complaint.GetComplaintsSites();

            bool hasCoordinates = false;
            if (sites != null)
            {
                for (int i = 0; i < sites.Count; i++)
                {
                    double lat, lon;
                    if (TryParseCoordinate(sites[i].latitude, out lat) && TryParseCoordinate(sites[i].longitude, out lon))
                    {
                        hasCoordinates = true;
                        break;
                    }
                }
            }

            if (!hasCoordinates)
            {
                MessageBox.Show("This complaint has no sites with valid coordinates to export.",
                    "Nothing to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Google Earth KMZ (*.kmz)|*.kmz";
            saveDialog.DefaultExt = "kmz";
            saveDialog.AddExtension = true;

            string suggestedName = complaint.GetComplaintId();
            if (string.IsNullOrEmpty(suggestedName))
                suggestedName = "complaint";
            foreach (char c in Path.GetInvalidFileNameChars())
                suggestedName = suggestedName.Replace(c, '_');
            saveDialog.FileName = suggestedName + ".kmz";

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string kml = BuildKml(complaint);
                WriteKmz(kml, saveDialog.FileName);

                MessageBox.Show("KMZ saved to:\n" + saveDialog.FileName,
                    "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to generate KMZ:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
