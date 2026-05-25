"""
Generate B&W GOST 19.701-90 compliant diagrams for the diploma project.
"""
import sys, os, math
sys.stdout.reconfigure(encoding='utf-8')
from PIL import Image, ImageDraw, ImageFont

OUT_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'Documents', 'diagrams')
os.makedirs(OUT_DIR, exist_ok=True)

try:
    FONT = ImageFont.truetype('arial.ttf', 13)
    FONT_SM = ImageFont.truetype('arial.ttf', 11)
    FONT_BOLD = ImageFont.truetype('arialbd.ttf', 13)
except:
    FONT = ImageFont.load_default()
    FONT_SM = FONT
    FONT_BOLD = FONT

# B&W palette
BLACK = '#000000'
WHITE = '#FFFFFF'
GRAY80 = '#CCCCCC'
GRAY60 = '#999999'
GRAY40 = '#666666'
LINE_W = 2


def draw_arrow(d, x1, y1, x2, y2):
    mid = (x1 + x2) // 2
    d.line([(x1, y1), (mid, y1), (mid, y2), (x2, y2)], fill=BLACK, width=LINE_W)


def draw_arrow_down(d, x, y1, y2):
    d.line([(x, y1), (x, y2)], fill=BLACK, width=LINE_W)
    angle = math.pi / 2
    mx, my = x, y2
    d.polygon([
        (mx, my), (mx - 6 * math.cos(angle - 0.4), my - 6 * math.sin(angle - 0.4)),
        (mx - 6 * math.cos(angle + 0.4), my - 6 * math.sin(angle + 0.4)),
    ], fill=BLACK)


def draw_centered(d, text, x, y, color=BLACK, font=None):
    if font is None: font = FONT
    bbox = d.textbbox((0, 0), text, font=font)
    tw = bbox[2] - bbox[0]
    th = bbox[3] - bbox[1]
    d.text((x - tw // 2, y - th // 2 - 1), text, fill=color, font=font)


def gost_flowchart(title, nodes, out_name):
    n = len(nodes)
    bw, bh = 180, 42
    gap = 24
    img_w = max(500, 240 + n * 5)
    img_h = 60 + n * (bh + gap) + 30
    img = Image.new('RGB', (img_w, img_h), WHITE)
    d = ImageDraw.Draw(img)

    # Title
    # draw_centered(d, title, img_w // 2, 18, BLACK, FONT_BOLD)

    for i, (ntype, text) in enumerate(nodes):
        cx = img_w // 2
        cy = 50 + i * (bh + gap)

        hw, hh = bw // 2, bh // 2

        if ntype in ('start', 'end'):
            rx, ry = hw, hh + 6
            d.rounded_rectangle([cx - rx, cy - ry, cx + rx, cy + ry], radius=16,
                                fill=WHITE, outline=BLACK, width=LINE_W)
            draw_centered(d, text, cx, cy, BLACK, FONT)

        elif ntype == 'decision':
            d.polygon([(cx, cy - hh - 4), (cx + hw + 10, cy),
                       (cx, cy + hh + 4), (cx - hw - 10, cy)],
                      fill=WHITE, outline=BLACK, width=LINE_W)
            draw_centered(d, text, cx, cy, BLACK, FONT_SM)

        elif ntype == 'io':
            pts = [(cx - hw - 12, cy - hh), (cx + hw - 12, cy - hh),
                   (cx + hw + 12, cy + hh), (cx - hw + 12, cy + hh)]
            d.polygon(pts, fill=WHITE, outline=BLACK, width=LINE_W)
            draw_centered(d, text, cx - 2, cy, BLACK, FONT_SM)

        elif ntype == 'subprocess':
            d.rectangle([cx - hw, cy - hh, cx + hw, cy + hh],
                        fill=WHITE, outline=BLACK, width=LINE_W)
            d.line([(cx - hw + 8, cy - hh), (cx - hw + 8, cy + hh)], fill=BLACK, width=LINE_W)
            d.line([(cx + hw - 8, cy - hh), (cx + hw - 8, cy + hh)], fill=BLACK, width=LINE_W)
            draw_centered(d, text, cx, cy, BLACK, FONT_SM)

        else:
            d.rectangle([cx - hw, cy - hh, cx + hw, cy + hh],
                        fill=WHITE, outline=BLACK, width=LINE_W)
            draw_centered(d, text, cx, cy, BLACK, FONT)

        if i < n - 1:
            next_cy = 50 + (i + 1) * (bh + gap)
            draw_arrow_down(d, cx, cy + hh, next_cy - bh // 2)

    path = os.path.join(OUT_DIR, out_name)
    img.save(path)
    return path


def er():
    rx, ry = 22, 18
    img = Image.new('RGB', (int(rx * 14), int(ry * 14) + 40), WHITE)
    d = ImageDraw.Draw(img)
    fw, fh = 130, 44

    entities = [
        ('Player', 40, 30),
        ('GameSession', 40, 100),
        ('Location', 260, 30),
        ('NPC', 260, 100),
        ('DialogNode', 260, 170),
        ('Item', 40, 170),
        ('Photo', 40, 240),
        ('Evidence', 40, 310),
        ('Trigger', 260, 310),
    ]

    boxes = {}
    for label, ex, ey in entities:
        x, y = ex, ey
        d.rectangle([x, y, x + fw, y + fh], fill=WHITE, outline=BLACK, width=LINE_W)
        d.line([(x, y + 22), (x + fw, y + 22)], fill=BLACK, width=1)
        draw_centered(d, label, x + fw // 2, y + 12, BLACK, FONT_BOLD)
        boxes[label] = (x + fw // 2, y + fh // 2)

    p = boxes['Player']
    s = boxes['GameSession']
    loc = boxes['Location']
    n = boxes['NPC']
    di = boxes['DialogNode']
    it = boxes['Item']
    ph = boxes['Photo']
    ev = boxes['Evidence']
    tr = boxes['Trigger']

    # Player - GameSession
    draw_arrow(d, p[0] + 65, p[1], s[0] - 65, s[1])
    # Player - Location
    draw_arrow(d, p[0] + 65, p[1] - 8, loc[0] - 65, loc[1])
    # Player - Item
    draw_arrow(d, p[0], p[1] + 22, it[0] + 65, it[1] - 22)
    # Player - Photo
    draw_arrow(d, p[0], p[1] + 22, ph[0] + 65, ph[1] - 22)
    # Player - NPC
    draw_arrow(d, p[0] + 65, p[1] + 8, n[0] - 65, n[1])
    # Location - NPC
    draw_arrow(d, loc[0], loc[1] + 22, n[0], n[1] - 22)
    # Location - Trigger
    draw_arrow(d, loc[0], loc[1] + 22, tr[0], tr[1] - 22)
    # NPC - DialogNode
    draw_arrow(d, n[0], n[1] + 22, di[0], di[1] - 22)
    # Item - Photo
    draw_arrow(d, it[0], it[1] + 22, ph[0], ph[1] - 22)
    # Photo - Evidence
    draw_arrow(d, ph[0], ph[1] + 22, ev[0], ev[1] - 22)

    path = os.path.join(OUT_DIR, 'er_diagram.png')
    img.save(path)
    return path


def architecture():
    img = Image.new('RGB', (700, 520), WHITE)
    d = ImageDraw.Draw(img)

    layers = [
        (60, 30, 580, 50, 'UI Layer (Canvas)'),
        (60, 95, 580, 70, 'Player: CameraController, PlayerMoveSystem, PlayerGUI'),
        (60, 180, 580, 70, 'Game Systems: DialogSystem, PhotoCamera,\nInventoryManager, TriggerSystem'),
        (60, 265, 580, 70, 'AI & Navigation: NPC pathfinding (NavMesh),\nWaypointPatrol'),
        (60, 350, 580, 50, 'Persistence: SaveSystem, BaseSaver components'),
        (60, 415, 580, 50, 'Core: EventManager, DependencyContainer,\nSceneLoader, ObjectPooler'),
    ]

    for lx, ly, lw, lh, ltext in layers:
        d.rectangle([lx, ly, lx + lw, ly + lh], fill=WHITE, outline=BLACK, width=LINE_W)
        lines = ltext.split('\n')
        line_h = 16
        start_y = ly + (lh - len(lines) * line_h) // 2
        for j, ln in enumerate(lines):
            draw_centered(d, ln, lx + lw // 2, start_y + j * line_h, BLACK, FONT_SM)

    # Layer labels on left
    labels = ['Интерфейс', 'Управление\nигроком', 'Игровые\nсистемы', 'AI и\nнавигация', 'Сохранение', 'Ядро']
    for i, lab in enumerate(layers):
        ly = lab[1]
        lh = lab[3]
        draw_centered(d, labels[i], 30, ly + lh // 2, GRAY60, FONT_SM)

    path = os.path.join(OUT_DIR, 'architecture.png')
    img.save(path)
    return path


if __name__ == '__main__':
    print('Generating ER diagram...')
    er()
    print('  Saved: er_diagram.png')

    print('Generating dialog system flowchart...')
    gost_flowchart('', [
        ('start', 'Начало'),
        ('process', 'Игрок подходит к NPC'),
        ('process', 'Загрузка DialogNode'),
        ('process', 'Отображение текста реплики'),
        ('decision', 'Есть варианты ответа?'),
        ('process', 'Отображение вариантов'),
        ('process', 'Игрок выбирает ответ'),
        ('process', 'Проверка условий'),
        ('decision', 'Следующий узел существует?'),
        ('process', 'Загрузка следующего узла'),
        ('process', 'Обновление состояния NPC'),
        ('decision', 'Диалог завершён?'),
        ('end', 'Конец'),
    ], 'flow_dialog.png')

    print('Generating photo camera flowchart...')
    gost_flowchart('', [
        ('start', 'Начало'),
        ('process', 'Экипировка фотоаппарата'),
        ('process', 'Активация HUD (прицел)'),
        ('process', 'Игрок наводит камеру'),
        ('process', 'Нажатие ЛКМ (снимок)'),
        ('process', 'Захват кадра (RenderTexture)'),
        ('process', 'Кодирование PNG в base64'),
        ('process', 'Сохранение в PhotoGallery'),
        ('decision', 'Локация «Завод»?'),
        ('process', 'Проверка стены с числами'),
        ('process', 'Проявление скрытых чисел'),
        ('process', 'Запись улики в инвентарь'),
        ('end', 'Конец'),
    ], 'flow_photo.png')

    print('Generating NPC behavior flowchart...')
    gost_flowchart('', [
        ('start', 'Начало'),
        ('process', 'Загрузка NPC в сцену'),
        ('decision', 'Тип NPC?'),
        ('process', 'Ожидание в точке (диалог)'),
        ('process', 'Патрулирование по точкам'),
        ('decision', 'Игрок в зоне обнаружения?'),
        ('process', 'Преследование игрока'),
        ('decision', 'Игрок скрылся?'),
        ('process', 'Возврат на маршрут'),
        ('decision', 'NPC жив?'),
        ('end', 'Конец'),
    ], 'flow_npc.png')

    print('Generating save/load flowchart...')
    gost_flowchart('', [
        ('start', 'Сохранение'),
        ('process', 'Игрок активирует сохранение'),
        ('process', 'Сбор данных через BaseSaver'),
        ('process', 'Сериализация в JSON'),
        ('process', 'Запись в файл .json'),
        ('end', 'Сохранено'),
        ('start', 'Загрузка'),
        ('process', 'Игрок выбирает файл'),
        ('process', 'Чтение файла и парсинг'),
        ('process', 'Загрузка сцены'),
        ('process', 'Восстановление состояния'),
        ('end', 'Загружено'),
    ], 'flow_save.png')

    print('Generating architecture diagram...')
    architecture()

    print('Generating player movement flowchart...')
    gost_flowchart('', [
        ('start', 'Начало'),
        ('process', 'Получение ввода (WASD)'),
        ('decision', 'Движение?'),
        ('process', 'Расчёт направления'),
        ('process', 'Применение скорости'),
        ('decision', 'Нажат Shift?'),
        ('process', 'Скорость = бег'),
        ('process', 'Скорость = ходьба'),
        ('decision', 'Нажат Ctrl?'),
        ('process', 'Приседание'),
        ('process', 'Обычная стойка'),
        ('process', 'Применение гравитации'),
        ('end', 'Конец кадра'),
    ], 'flow_player.png')

    print('Generating interaction flowchart...')
    gost_flowchart('', [
        ('start', 'Начало'),
        ('process', 'Raycast из центра экрана'),
        ('decision', 'Попадание в Interactable?'),
        ('process', 'Отображение подсказки'),
        ('decision', 'Нажата клавиша E?'),
        ('process', 'Вызов Interact()'),
        ('decision', 'Тип объекта:'),
        ('process', 'ItemPickup: в инвентарь'),
        ('process', 'DialogTrigger: диалог'),
        ('process', 'SceneLoader: сцена'),
        ('process', 'ReadableItem: текст'),
        ('process', 'Скрытие подсказки'),
        ('end', 'Конец'),
    ], 'flow_interact.png')

    print('All diagrams generated!')
