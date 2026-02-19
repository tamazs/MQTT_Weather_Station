DO $EF$
BEGIN
          IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'iot_weatherstation') THEN
CREATE SCHEMA iot_weatherstation;
END IF;
END $EF$;


CREATE TABLE iot_weatherstation."Measurements" (
                                                   "Id" uuid NOT NULL,
                                                   "StationId" text NOT NULL,
                                                   "SensorId" text NOT NULL,
                                                   "Timestamp" timestamp with time zone NOT NULL,
                                                   "Temperature" double precision NOT NULL,
                                                   "Humidity" double precision NOT NULL,
                                                   "Pressure" double precision NOT NULL,
                                                   "LightLevel" integer NOT NULL,
                                                   CONSTRAINT "PK_Measurements" PRIMARY KEY ("Id")
);