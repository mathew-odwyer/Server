FROM node:18

EXPOSE 8080

WORKDIR /app

RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

COPY package*.json ./

RUN npm install

COPY gateway gateway

CMD ["npm", "run", "start-gateway"]