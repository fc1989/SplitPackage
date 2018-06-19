FROM nginx AS base
EXPOSE 80

FROM node:latest AS build
WORKDIR /app
COPY . .
RUN npm install \
&& npm run buildtest

FROM base AS final
COPY --from=build /app/dist/test/ /usr/share/nginx/html