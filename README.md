# RabbitDemo
Два проекта. Publisher и Subscriber

Запускаютcя по разным порты. Все в launchSettings. Там же  и демо параметры соединения к RabbitMQ, имена Exchange и Queue. 

Самый первый раз Subscriber нужно запускать первым, ибо он создает Exchange и Queue.

Publisher поднимает сваггер с одним методом которым и шлет сообщение.

Sibscriber слушает и пишет в консоль, что пришло.

Rabbit докер брал прямо из коробки, ничего не настраивал
https://www.rabbitmq.com/download.html

