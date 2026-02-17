import xml.etree.ElementTree as ET

file = r'C:\Users\Public\Repositorio\AcademyIO_CI_CD_Doker_Kubernets_Modulo5\tests\AcademyIO.Tests\TestResults\3c5d3f27-ec84-4139-9ec8-5c909606d522\coverage.cobertura.xml'
tree = ET.parse(file)
root = tree.getroot()

classes_found = {}

for cls in root.findall('.//class'):
    name = cls.get('name', '')
    fn = cls.get('filename', '')
    if 'AcademyIO.Courses.API' not in fn:
        continue
    
    # Check for specific classes
    if any(x in name for x in ['CourseQuery', 'LessonQuery', 'LessonCommandHandler', 'ProgressLesson']):
        lines = len(cls.findall('.//line'))
        covered = sum(1 for line in cls.findall('.//line') if int(line.get('hits', 0)) > 0)
        classes_found[name.split('.')[-1]] = {
            'name': name,
            'fn': fn,
            'total': lines,
            'covered': covered
        }

print('Found classes from new tests:')
for cls_short, info in classes_found.items():
    pct = (info['covered'] / info['total'] * 100) if info['total'] > 0 else 0
    print(f'  {cls_short}: {info["covered"]}/{info["total"]} = {pct:.1f}%')

if not classes_found:
    print('  None found!')
