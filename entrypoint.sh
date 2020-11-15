#!/bin/bash

/app/proxy &
dotnet /app/app.dll
