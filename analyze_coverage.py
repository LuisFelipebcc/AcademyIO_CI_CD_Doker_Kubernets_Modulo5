import xml.etree.ElementTree as ET

file = r'C:\Users\Public\Repositorio\AcademyIO_CI_CD_Doker_Kubernets_Modulo5\tests\AcademyIO.Tests\TestResults\0548ac1d-0a7a-40ab-8f6a-7500105761cb\coverage.cobertura.xml'
tree = ET.parse(file)
root = tree.getroot()

classes_coverage = {}
for pkg in root.findall('.//class'):
    name = pkg.get('name', '')
    if 'AcademyIO.Courses.API' not in name or '<>' in name or 'd__' in name:
        continue
    
    lines = pkg.findall('.//line')
    if not lines:
        continue
    
    total = len(lines)
    covered = sum(1 for line in lines if int(line.get('hits', 0)) > 0)
    percentage = (covered / total * 100) if total > 0 else 0
    
    classes_coverage[name] = {'total': total, 'covered': covered, 'percentage': percentage}

low_coverage = sorted([(k, v) for k, v in classes_coverage.items() if v['percentage'] < 100], key=lambda x: x[1]['percentage'])

print('Classes with < 100% coverage:')
print('-' * 90)
for name, data in low_coverage[:30]:
    short_name = name.split('.')[-1][:50]
    print(f'{short_name:50s} {data["total"]:5d} total, {data["covered"]:5d} covered, {data["percentage"]:6.1f}%')
