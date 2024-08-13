ARG ADD_TRUSTED_CERT=false

# Based on Defra's dotnet image
FROM defradigital/dotnetcore-development:dotnet6.0 AS build-with-trusted-cert-false

# This is for building the docker base on a host machine behind a Corporate proxy or VPN
# we need to add in the corporate certificate chain
# e.g. Kainos developers will need the Kainos ZScaler trusted certificate
FROM defradigital/dotnetcore-development:dotnet6.0 AS build-with-trusted-cert-true
USER root
ARG TRUSTED_CERT
COPY ./${TRUSTED_CERT} /usr/local/share/ca-certificates/
RUN if [[ -z "$TRUSTED_CERT" ]] ;\
        then echo TRUSTED_CERT not provided;\
        else cat /usr/local/share/ca-certificates/${TRUSTED_CERT} >> /etc/ssl/certs/ca-certificates.crt ;\
        fi

FROM build-with-trusted-cert-${ADD_TRUSTED_CERT} AS build
USER root
RUN apk --no-cache add nodejs npm
USER dotnet
WORKDIR /src
COPY ["FrontendObligationChecker/", "FrontendObligationChecker/"]
RUN dotnet restore "FrontendObligationChecker/FrontendObligationChecker.csproj"
WORKDIR "/src/FrontendObligationChecker"
RUN dotnet build "FrontendObligationChecker.csproj" -c Release -o /home/dotnet/build

FROM build AS publish
RUN dotnet publish "FrontendObligationChecker.csproj" -c Release -o /home/dotnet/publish

# Based on Defra's dotnet image
FROM defradigital/dotnetcore:dotnet6.0 AS base-with-trusted-cert-false
USER root

# This is for building the docker base on a host machine behind a Corporate proxy or VPN
# we need to add in the corporate certificate chain
# e.g. Kainos developers will need the Kainos ZScaler trusted certificate
FROM defradigital/dotnetcore:dotnet6.0 AS base-with-trusted-cert-true
USER root
ARG TRUSTED_CERT
COPY ./${TRUSTED_CERT} /usr/local/share/ca-certificates/
RUN if [[ -z "$TRUSTED_CERT" ]] ;\
        then echo TRUSTED_CERT not provided;\
        else cat /usr/local/share/ca-certificates/${TRUSTED_CERT} >> /etc/ssl/certs/ca-certificates.crt ;\
        fi

# Installs globalization tools and sets common environment settings
FROM base-with-trusted-cert-${ADD_TRUSTED_CERT} AS configured-base
RUN apk --no-cache add icu-libs icu-data-full libintl
USER dotnet
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM configured-base AS final
ARG PORT=3000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}
WORKDIR /app
COPY --from=publish /home/dotnet/publish .
ENTRYPOINT ["dotnet", "FrontendObligationChecker.dll"]
