FROM nginx
COPY dist /usr/share/nginx/html
COPY innodisk.com.crt /etc/nginx/innodisk.com.crt
COPY innodisk.com.key /etc/nginx/innodisk.com.key
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443
