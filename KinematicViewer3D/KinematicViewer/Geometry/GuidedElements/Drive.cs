﻿using KinematicViewer.Geometry.Figures;
using KinematicViewer.Geometry.Guides;
using KinematicViewer.Transformation;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer.Geometry.GuidedElements
{
    public class Drive : GeometricalElement
    {
        private double _dExtractedLength;
        private double _dOffsetSecondCylinder;
        private double _dRetractedLength;
        private double _dStroke;
        private int _iRadiusBody;
        private int _iRadiusDoor;

        private Material _oBodyPartMaterial;
        private Material _oBodyPartMaterialStartPoint;
        private Material _oDoorPartMaterialEndPoint;
        private Material _oDoorPartMaterialExtracted;
        private Material _oDoorPartMaterialMiddle;
        private Material _oDoorPartMaterialRetracted;
        private Point3D _oEndPoint;
        private Point3D _oStartPoint;
        private Vector3D _oVDrive;

        /// <summary>
        /// Erzeugt ein visuelles Modell eines Antriebs
        /// </summary>
        /// <param name="point1">Start Punkt des Antriebs</param>
        /// <param name="point2">End Punkt des Antriebs</param>
        /// <param name="name">Name des Objektes</param>
        /// <param name="mat">Oberflächenmaterial des Antriebs</param>
        public Drive(Point3D point1, Point3D point2, string name = "Drive", Material mat = null)
            : base(mat)
        {
            StartPoint = point1;
            EndPoint = point2;

            RadiusBody = 15;
            RadiusDoor = 25;

            VDrive = EndPoint - StartPoint;
            RetractedLenght = VDrive.Length;
            ExtractedLength = 648.8;
            Stroke = _dExtractedLength - _dRetractedLength;
            OffsetSecondCylinder = (0.25 * VDrive).Length;
            Name = name;

            BodyPartMaterial = new DiffuseMaterial(Brushes.Gray);
            BodyPartMaterialStartPoint = new DiffuseMaterial(Brushes.Gray);
            DoorPartMaterialEndPoint = new DiffuseMaterial(Brushes.Gray);
            DoorPartMaterialRetracted = new DiffuseMaterial(Brushes.YellowGreen);
            DoorPartMaterialMiddle = new DiffuseMaterial(Brushes.Orange);
            DoorPartMaterialExtracted = new DiffuseMaterial(Brushes.OrangeRed);
        }

        public Material BodyPartMaterial
        {
            get { return _oBodyPartMaterial; }
            set { _oBodyPartMaterial = value; }
        }

        public Material BodyPartMaterialStartPoint
        {
            get { return _oBodyPartMaterialStartPoint; }
            set { _oBodyPartMaterialStartPoint = value; }
        }

        public Material DoorPartMaterialEndPoint
        {
            get { return _oDoorPartMaterialEndPoint; }
            set { _oDoorPartMaterialEndPoint = value; }
        }

        public Material DoorPartMaterialExtracted
        {
            get { return _oDoorPartMaterialExtracted; }
            set { _oDoorPartMaterialExtracted = value; }
        }

        public Material DoorPartMaterialMiddle
        {
            get { return _oDoorPartMaterialMiddle; }
            set { _oDoorPartMaterialMiddle = value; }
        }

        public Material DoorPartMaterialRetracted
        {
            get { return _oDoorPartMaterialRetracted; }
            set { _oDoorPartMaterialRetracted = value; }
        }

        public Point3D EndPoint
        {
            get { return _oEndPoint; }
            set { _oEndPoint = value; }
        }

        public double ExtractedLength
        {
            get { return _dExtractedLength; }
            private set { _dExtractedLength = value; }
        }

        public double OffsetSecondCylinder
        {
            get { return _dOffsetSecondCylinder; }
            private set { _dOffsetSecondCylinder = value; }
        }

        public int RadiusBody
        {
            get { return _iRadiusBody; }
            private set { _iRadiusBody = value; }
        }

        public int RadiusDoor
        {
            get { return _iRadiusDoor; }
            private set { _iRadiusDoor = value; }
        }

        public double RetractedLenght
        {
            get { return _dRetractedLength; }
            private set { _dRetractedLength = value; }
        }

        public Point3D StartPoint
        {
            get { return _oStartPoint; }
            set { _oStartPoint = value; }
        }

        public double Stroke
        {
            get { return _dStroke; }
            private set { _dStroke = value; }
        }

        public Vector3D VDrive
        {
            get { return _oVDrive; }
            set { _oVDrive = value; }
        }

        public Drive generateReflectedDrive(Point3D SP, Vector3D RV)
        {
            Point3D p1 = TransformationUtilities.reflectPoint(SP, RV, StartPoint);
            Point3D p2 = TransformationUtilities.reflectPoint(SP, RV, EndPoint);

            return new Drive(p1, p2);
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoor = guide.MovePoint(EndPoint);

            Vector3D vDriveUpdated = attPointDoor - StartPoint;
            double vLength = vDriveUpdated.Length;
            vDriveUpdated.Normalize();
            //vDriveUpdated = TransformationUtilities.ScaleVector(vDriveUpdated, 1);

            //Offset um welchen der zweite Cylinder verschoben wird
            Vector3D vOffset = (OffsetSecondCylinder + (vLength - VDrive.Length)) * vDriveUpdated;

            //grüner Bereich => retracted
            if (vLength <= ExtractedLength - Stroke * 2 / 3)
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, BodyPartMaterial).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, 16, 16, BodyPartMaterialStartPoint).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, DoorPartMaterialRetracted).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, 16, 16, DoorPartMaterialEndPoint).GetGeometryModel(guide));
            }

            //Gelber Bereich => middle
            if ((vLength > ExtractedLength - Stroke * 2 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, BodyPartMaterial).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, 16, 16, BodyPartMaterialStartPoint).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, DoorPartMaterialMiddle).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, 16, 16, DoorPartMaterialEndPoint).GetGeometryModel(guide));
            }

            //Roter Bereich => extracted
            if ((vLength > ExtractedLength - Stroke * 1 / 3) && (vLength > ExtractedLength - Stroke * 2 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, BodyPartMaterial).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, 16, 16, BodyPartMaterialStartPoint).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, DoorPartMaterialExtracted).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, 16, 16, DoorPartMaterialEndPoint).GetGeometryModel(guide));
            }

            //Farblicher Anbindungspunkt an die Heckklappe in Farbe der Heckklappe (Standardmaterialfarbe Cyan)
            Res.AddRange(new Sphere(attPointDoor, 40, 16, 16, Material).GetGeometryModel(guide));

            return Res.ToArray();
        }

        public LineOfAction GetLever(Point3D point3D, Vector3D vector3D)
        {
            return new LineOfAction(
                point3D,
                this.StartPoint,
                this.EndPoint,
                vector3D);
        }
    }
}