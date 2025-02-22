# План проспект

## Введение

Соответствие статьи научному стилю является одним из основных критериев
принятия статьи к публикации. В текущем виде, процесс проверки представляет собой
отправку статьи на рецензирование, ожидание ответа, исправление недочетов и отправка на повторную проверку – данные этапы могут занимать достаточно много времени.
В связи с этим, автоматизация данного процесса является актуальной задачей, позволяющей значительно ускорить процесс выявления ошибок для исправления, и в следствие этого ускорить сам процесс публикации статьи, а также ускорить обучение начинающих авторов. В соответствие с этим возникает задача исследования возможности
автоматизации процесса проверки научных статей на соответствие научному стилю

## Обзор предметной области

### Основные понятия

Научный стиль - наиболее строгий стиль речи, используемый для написания
научных статей. Характеризуется использованием научной терминологии, исключая
жаргонизмы. Научный стиль не допускает личного изложения [1]. Проверяя текст на
соответствие научному стилю, следует в первую очередь реализовать и базовую проверку на качество текста. К такого рода анализу можно отнести SEO-анализ. SEO
(search engine optimization) анализ [2-3] популярен и актуален в связи с необходимостью продвижения ресурсов, товаров и услуг в сети Интернет. SEO анализ текста дает
возможность понять, насколько часто употребляются ключевые слова в тексте, как
много в тексте слов, не имеющих смысловой нагрузки и другое.
SEO-анализе вводит следующие термины для двух критериев, которые проверяются в данной работе:
Тошнота – это показатель повторений в текстовом документе ключевых слов и
фраз. Синонимом тошноты является термин плотность [3]. Вода - процентное соотношение стоп-слов и общего количества слов в тексте [3]. Так как эти критерии вычисляемы, то можно автоматизировать их получение.
Так же существует эмпирическая закономерность распределения частоты слов
естественного языка - Закон Ципфа: если все слова языка или достаточно длинного текста упорядочить по убыванию частоты их использования, то частота n-го слова в таком
списке окажется приблизительно обратно пропорциональной его порядковому номеру
n [4-5]. Соответствие распределения слов в тексте закону Ципфа говорит об уровне его
естественности. Расчет этого критерия так же можно автоматизировать. В предыдущей 
работе [7] был проведен более детальный обзор пригодности данных критериев к задачам автоматической проверки качества стиля статей.
Помимо описанных числовых критериев важными показателями качества научной статьи являются её экспертность и полезность. На данный момент верификация
этих критериев возможна только силами человека, однако ведутся разработки инструментов, способных выполнить данную задачу с помощью методов машинного обучения [6]. Недостатком подобных систем является сложность настройки, необходимость
больших обучающих выборок и узкая ориентация в смысле предметной области. 

### Обзор аналогов

Существуют вебсервисы, проверяющие текст по этим критериям - сервисы, позволяющие провести SEOанализ текста, например Анализатор качества контента 1y.ru [5], сервис проверки текстов
text.ru [6], сервис, осуществляющий поиск стоп-слов и подсчет их процентного соотношения
к общей длине текста contentmonster.ru [7].
Сравнение аналогов будет проводиться по следующим критериям:
• Многокритериальная проверка - как много критериев проверки использует
сервис;
• Ограничение длины текста - отсутствие ограничения длины текста,
поступающего на проверку;
• Проверка научного стиля - проверка текста на соответствие научному стилю.
В табл.1 представлено сравнение аналогов.
Таблица

<ДОБАВИТЬ ЕЩЕ АНАЛОГОВ>

## Выбор метода решения

### Виды решений

Для быстрого анализа статей, и в том числе для исследования решения принятл решение
реализовать исполняемый сценарий.
Для удобства пользователей, и в качестве решения принято реализовать веб-сервис.
<Расписать подробнее, может таблиц добавить>

### Используемые технологии

<про python быстро для скрипта, про .net core т.к. кроссплатформ, развивающийся>

#### Выбор технологии фронтенда

<сравнение вариантов под .net core, в районе 4-5>

## Описание метода решения

### Описание архитектуры решения

### Описание алгоритмов работы

### Жизненные цикл анализа статьи

<Исполняемые сценарии, работа пользователя>

## Исследование решения

В рамках исследования проверялась гипотеза о том, что качество научной статьи
влияет на значения ранее определенных числовых критериев, а также то, что полученная выборка значений будет соответствовать нормальному распределению.
Исследование на выборке из 2500 прошедших рецензирование и опубликованных
статей позволит получить математические параметры распределений, что позволит
установить пороговые значения числовых критериев для статей хорошего качества

<дальше по статье из итмо>


## Вывод


