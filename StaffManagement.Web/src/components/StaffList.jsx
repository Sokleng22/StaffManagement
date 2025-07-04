import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Button,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Alert,
  Chip,
  IconButton,
  Tooltip
} from '@mui/material';
import { Grid } from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Search as SearchIcon,
  Download as DownloadIcon,
  Refresh as RefreshIcon
} from '@mui/icons-material';
import { DataGrid } from '@mui/x-data-grid';
import { staffApi } from '../services/staffApi';
import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import 'jspdf-autotable';

const StaffList = () => {
  const navigate = useNavigate();
  const [staffData, setStaffData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [department, setDepartment] = useState('');
  const [position, setPosition] = useState('');
  const [isActive, setIsActive] = useState(null);
  const [departments, setDepartments] = useState([]);
  const [positions, setPositions] = useState([]);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [staffToDelete, setStaffToDelete] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleCloseSnackbar = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  const loadStaff = useCallback(async () => {
    setLoading(true);
    try {
      const params = {
        page: page + 1, // API uses 1-based indexing
        pageSize,
        searchTerm: searchTerm || undefined,
        department: department || undefined,
        position: position || undefined,
        isActive: isActive,
        sortBy: 'Id',
        sortDirection: 'asc'
      };

      const result = await staffApi.getAll(params);
      setStaffData(result.items);
      setTotalRows(result.totalCount);
    } catch (error) {
      console.error('Error loading staff:', error);
      showSnackbar('Error loading staff data', 'error');
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, searchTerm, department, position, isActive]);

  const loadDepartmentsAndPositions = async () => {
    try {
      const [deptData, posData] = await Promise.all([
        staffApi.getDepartments(),
        staffApi.getPositions()
      ]);
      setDepartments(deptData);
      setPositions(posData);
    } catch (error) {
      console.error('Error loading departments/positions:', error);
    }
  };

  useEffect(() => {
    loadStaff();
  }, [loadStaff]);

  useEffect(() => {
    loadDepartmentsAndPositions();
  }, []);

  const handleEdit = (id) => {
    navigate(`/staff/edit/${id}`);
  };

  const handleDelete = async () => {
    if (!staffToDelete) return;

    try {
      await staffApi.delete(staffToDelete.id);
      showSnackbar('Staff member deleted successfully');
      loadStaff();
    } catch (error) {
      console.error('Error deleting staff:', error);
      showSnackbar('Error deleting staff member', 'error');
    } finally {
      setDeleteDialogOpen(false);
      setStaffToDelete(null);
    }
  };

  const handleExportCsv = async () => {
    try {
      const blob = await staffApi.exportToCsv();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `staff_export_${new Date().toISOString().split('T')[0]}.csv`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
      showSnackbar('CSV export completed');
    } catch (error) {
      console.error('Error exporting CSV:', error);
      showSnackbar('Error exporting CSV', 'error');
    }
  };

  const handleExportExcel = () => {
    const worksheet = XLSX.utils.json_to_sheet(staffData.map(staff => ({
      ID: staff.id,
      'First Name': staff.firstName,
      'Last Name': staff.lastName,
      Email: staff.email,
      Phone: staff.phone || '',
      Department: staff.department,
      Position: staff.position,
      Salary: staff.salary,
      'Hire Date': new Date(staff.hireDate).toLocaleDateString(),
      Status: staff.isActive ? 'Active' : 'Inactive'
    })));

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Staff');
    XLSX.writeFile(workbook, `staff_export_${new Date().toISOString().split('T')[0]}.xlsx`);
    showSnackbar('Excel export completed');
  };

  const handleExportPdf = () => {
    try {
      // Check if there's data to export
      if (!staffData || staffData.length === 0) {
        showSnackbar('No data available to export', 'warning');
        return;
      }

      console.log('Starting PDF export with data:', staffData.length, 'records');
      
      const doc = new jsPDF();
      
      doc.setFontSize(20);
      doc.text('Staff Management Report', 20, 20);
      
      doc.setFontSize(12);
      doc.text(`Generated on: ${new Date().toLocaleDateString()}`, 20, 30);
      
      const tableData = staffData.map(staff => [
        staff.id,
        staff.fullName || `${staff.firstName} ${staff.lastName}`,
        staff.email,
        staff.department,
        staff.position,
        `$${staff.salary?.toLocaleString() || '0'}`,
        new Date(staff.hireDate).toLocaleDateString(),
        staff.isActive ? 'Active' : 'Inactive'
      ]);

      console.log('Table data prepared:', tableData.length, 'rows');

      doc.autoTable({
        head: [['ID', 'Name', 'Email', 'Department', 'Position', 'Salary', 'Hire Date', 'Status']],
        body: tableData,
        startY: 40,
        styles: { fontSize: 8 },
        headStyles: { fillColor: [25, 118, 210] }
      });

      const filename = `staff_report_${new Date().toISOString().split('T')[0]}.pdf`;
      console.log('Saving PDF as:', filename);
      
      doc.save(filename);
      showSnackbar('PDF export completed');
      console.log('PDF export successful');
      
    } catch (error) {
      console.error('Error exporting PDF:', error);
      showSnackbar(`Error exporting PDF: ${error.message}`, 'error');
    }
  };

  const handleClearFilters = () => {
    setSearchTerm('');
    setDepartment('');
    setPosition('');
    setIsActive(null);
  };

  const columns = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'fullName', headerName: 'Full Name', width: 200 },
    { field: 'email', headerName: 'Email', width: 250 },
    { field: 'phone', headerName: 'Phone', width: 150 },
    { field: 'department', headerName: 'Department', width: 150 },
    { field: 'position', headerName: 'Position', width: 180 },
    { 
      field: 'salary', 
      headerName: 'Salary', 
      width: 120,
      renderCell: (params) => `$${params.value.toLocaleString()}`
    },
    { 
      field: 'hireDate', 
      headerName: 'Hire Date', 
      width: 120,
      renderCell: (params) => new Date(params.value).toLocaleDateString()
    },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 100,
      renderCell: (params) => (
        <Chip
          label={params.value ? 'Active' : 'Inactive'}
          color={params.value ? 'success' : 'default'}
          size="small"
        />
      )
    },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 120,
      sortable: false,
      renderCell: (params) => (
        <Box>
          <Tooltip title="Edit">
            <IconButton size="small" onClick={() => handleEdit(params.row.id)}>
              <EditIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete">
            <IconButton 
              size="small" 
              onClick={() => {
                setStaffToDelete(params.row);
                setDeleteDialogOpen(true);
              }}
            >
              <DeleteIcon />
            </IconButton>
          </Tooltip>
        </Box>
      )
    }
  ];

  return (
    <Box>
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
          <Typography variant="h4" component="h1">
            Staff Management
          </Typography>
          <Box>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => navigate('/staff/add')}
              sx={{ mr: 1 }}
            >
              Add Staff
            </Button>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon />}
              onClick={loadStaff}
              sx={{ mr: 1 }}
            >
              Refresh
            </Button>
            <Button
              variant="outlined"
              startIcon={<DownloadIcon />}
              onClick={handleExportCsv}
              sx={{ mr: 1 }}
            >
              CSV
            </Button>
            <Button
              variant="outlined"
              startIcon={<DownloadIcon />}
              onClick={handleExportExcel}
              sx={{ mr: 1 }}
            >
              Excel
            </Button>
            <Button
              variant="outlined"
              startIcon={<DownloadIcon />}
              onClick={handleExportPdf}
            >
              PDF
            </Button>
          </Box>
        </Box>

        {/* Filters */}
        <Grid container spacing={2} sx={{ mb: 3 }}>
          <Grid item xs={12} md={3}>
            <TextField
              fullWidth
              label="Search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                startAdornment: <SearchIcon sx={{ mr: 1, color: 'action.active' }} />
              }}
            />
          </Grid>
          <Grid item xs={12} md={2}>
            <FormControl fullWidth>
              <InputLabel>Department</InputLabel>
              <Select
                value={department}
                label="Department"
                onChange={(e) => setDepartment(e.target.value)}
              >
                <MenuItem value="">All</MenuItem>
                {departments.map((dept) => (
                  <MenuItem key={dept} value={dept}>{dept}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={2}>
            <FormControl fullWidth>
              <InputLabel>Position</InputLabel>
              <Select
                value={position}
                label="Position"
                onChange={(e) => setPosition(e.target.value)}
              >
                <MenuItem value="">All</MenuItem>
                {positions.map((pos) => (
                  <MenuItem key={pos} value={pos}>{pos}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={2}>
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={isActive === null ? '' : isActive}
                label="Status"
                onChange={(e) => setIsActive(e.target.value === '' ? null : e.target.value)}
              >
                <MenuItem value="">All</MenuItem>
                <MenuItem value={true}>Active</MenuItem>
                <MenuItem value={false}>Inactive</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={3}>
            <Button
              variant="outlined"
              onClick={handleClearFilters}
              fullWidth
              sx={{ height: '56px' }}
            >
              Clear Filters
            </Button>
          </Grid>
        </Grid>

        {/* Data Grid */}
        <Box sx={{ height: 600, width: '100%' }}>
          <DataGrid
            rows={staffData}
            columns={columns}
            loading={loading}
            pageSizeOptions={[5, 10, 25, 50]}
            paginationModel={{ page, pageSize }}
            onPaginationModelChange={(model) => {
              setPage(model.page);
              setPageSize(model.pageSize);
            }}
            rowCount={totalRows}
            paginationMode="server"
            disableRowSelectionOnClick
            sx={{
              '& .MuiDataGrid-row:hover': {
                backgroundColor: 'action.hover',
              },
            }}
          />
        </Box>
      </Paper>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          Are you sure you want to delete {staffToDelete?.fullName}? This action cannot be undone.
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar for notifications */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
      >
        <Alert onClose={handleCloseSnackbar} severity={snackbar.severity}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default StaffList;
