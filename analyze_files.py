import xml.etree.ElementTree as ET

file = r'C:\Users\Public\Repositorio\AcademyIO_CI_CD_Doker_Kubernets_Modulo5\tests\AcademyIO.Tests\TestResults\26614fdc-2b25-4539-ad13-fb210bbe1b1d\coverage.cobertura.xml'
tree = ET.parse(file)
root = tree.getroot()

files_coverage = {}

# Filter only AcademyIO.Courses.API
for cls in root.findall('.//class'):
    fn = cls.get('filename', '')
    if 'AcademyIO.Courses.API' not in fn:
        continue
    
    # Extract file path  
    if '/src/' in fn:
        fn_short = fn.split('/src/')[1]
    else:
        fn_short = fn
    
    lines = cls.findall('.//line')
    total = len(lines)
    covered = sum(1 for line in lines if int(line.get('hits', 0)) > 0)
    
    if fn_short not in files_coverage:
        files_coverage[fn_short] = {'total': 0, 'covered': 0}
    
    files_coverage[fn_short]['total'] += total
    files_coverage[fn_short]['covered'] += covered

# Sort by coverage percentage (ascending)
sorted_files = sorted(
    [(f, v) for f, v in files_coverage.items()],
    key=lambda x: (x[1]['covered'] / x[1]['total']) if x[1]['total'] > 0 else 0
)

print('File Coverage Summary (sorted by lowest coverage):')
print('-' * 120)
for fn, data in sorted_files:
    pct = (data['covered'] / data['total'] * 100) if data['total'] > 0 else 0
    print(f'{fn:70s} {data["covered"]:5d}/{data["total"]:5d} = {pct:6.1f}%')
