FROM mcr.microsoft.com/dotnet/core/sdk:3.1.301 as app

COPY app app

RUN dotnet publish -c Release -o /app app

FROM golang:alpine as proxy

RUN apk update && apk add --no-cache git
WORKDIR $GOPATH/src

COPY . .

WORKDIR $GOPATH/src/proxy

RUN go get -d -v
RUN GOOS=linux GOARCH=amd64 CGO_ENABLED=0 go build -ldflags="-w -s" -o /proxy

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.5-bionic
RUN apt update && apt install -y dos2unix

ENTRYPOINT ["/entrypoint.sh"]

COPY entrypoint.sh entrypoint.sh
RUN dos2unix entrypoint.sh && chmod +x entrypoint.sh

COPY --from=app /app /app
COPY --from=proxy /proxy /app/proxy
